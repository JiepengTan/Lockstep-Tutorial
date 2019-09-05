using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Lockstep.Network {
    public static class OpcodeHelper {
        private static readonly HashSet<ushort> needDebugLogMessageSet = new HashSet<ushort> {1};

        public static bool IsNeedDebugLogMessage(ushort opcode){
            if (opcode > 1000) {
                return true;
            }

            if (needDebugLogMessageSet.Contains(opcode)) {
                return true;
            }

            return false;
        }

        public static bool IsClientHotfixMessage(ushort opcode){
            return opcode > 10000;
        }
    }

    public static class NetworkUtil {
        public static IPEndPoint ToIPEndPoint(string host, int port){
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static IPEndPoint ToIPEndPoint(string address){
            int index = address.LastIndexOf(':');
            string host = address.Substring(0, index);
            string p = address.Substring(index + 1);
            int port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }
    }

    public class NetInnerProxy : NetworkProxy {
        public readonly Dictionary<IPEndPoint, Session> adressSessions = new Dictionary<IPEndPoint, Session>();

        public override void Remove(long id){
            Session session = this.Get(id);
            if (session == null) {
                return;
            }

            this.adressSessions.Remove(session.RemoteAddress);

            base.Remove(id);
        }

        /// <summary>
        /// 从地址缓存中取Session,如果没有则创建一个新的Session,并且保存到地址缓存中
        /// </summary>
        public Session Get(IPEndPoint ipEndPoint){
            if (this.adressSessions.TryGetValue(ipEndPoint, out Session session)) {
                return session;
            }

            session = this.Create(ipEndPoint);

            this.adressSessions.Add(ipEndPoint, session);
            return session;
        }
    }

    public class NetOuterProxy : NetworkProxy { }

    public abstract class NetworkProxy : NetBase {
        private AService Service;

        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

        public IMessagePacker MessagePacker { get; set; }

        public IMessageDispatcher MessageDispatcher { get; set; }

        public void Awake(NetworkProtocol protocol){
            switch (protocol) {
                case NetworkProtocol.TCP:
                    this.Service = new TService();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Awake(NetworkProtocol protocol, IPEndPoint ipEndPoint){
            try {
                switch (protocol) {
                    case NetworkProtocol.TCP:
                        this.Service = new TService(ipEndPoint);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.StartAccept();
            }
            catch (Exception e) {
                throw new Exception($"{ipEndPoint}", e);
            }
        }

        private async void StartAccept(){
            while (true) {
                if (this.IsDisposed) {
                    return;
                }

                await this.Accept();
            }
        }

        public virtual async Task<Session> Accept(){
            AChannel channel = await this.Service.AcceptChannel();
            Session session = CreateSession(this, channel);
            channel.ErrorCallback += (c, e) => { this.Remove(session.Id); };
            this.sessions.Add(session.Id, session);
            session.Start();
            return session;
        }

        public virtual void Remove(long id){
            Session session;
            if (!this.sessions.TryGetValue(id, out session)) {
                return;
            }

            this.sessions.Remove(id);
            session.Dispose();
        }

        public Session Get(long id){
            Session session;
            this.sessions.TryGetValue(id, out session);
            return session;
        }

        public static Session CreateSession(NetworkProxy net, AChannel c){
            Session session = new Session {Id = IdGenerater.GenerateId()};
            session.Awake(net, c);
            return session;
        }

        /// <summary>
        /// 创建一个新Session
        /// </summary>
        public virtual Session Create(IPEndPoint ipEndPoint){
            try {
                AChannel channel = this.Service.ConnectChannel(ipEndPoint);
                Session session = CreateSession(this, channel);
                channel.ErrorCallback += (c, e) => { this.Remove(session.Id); };
                this.sessions.Add(session.Id, session);
                return session;
            }
            catch (Exception e) {
                Log.Error(e.ToString());
                return null;
            }
        }

        public void Update(){
            if (this.Service == null) {
                return;
            }

            this.Service.Update();
        }

        public override void Dispose(){
            if (this.IsDisposed) {
                return;
            }

            base.Dispose();

            foreach (Session session in this.sessions.Values.ToArray()) {
                session.Dispose();
            }

            this.Service.Dispose();
        }
    }


    public sealed class Session : NetBase {
        private static int RpcId { get; set; }
        private AChannel channel;

        private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();
        private readonly List<byte[]> byteses = new List<byte[]>() {new byte[1], new byte[0], new byte[0]};

        public NetworkProxy Network;
        public object BindInfo;

        public T GetBindInfo<T>() where T : class{
            return BindInfo as T;
        }

        public void Awake(NetworkProxy net, AChannel c){
            this.Network = net;
            this.channel = c;
            this.requestCallback.Clear();
        }

        public void Start(){
            this.StartRecv();
        }

        public override void Dispose(){
            if (this.IsDisposed) {
                return;
            }

            long id = this.Id;

            base.Dispose();

            foreach (Action<IResponse> action in this.requestCallback.Values.ToArray()) {
                action.Invoke(new ResponseMessage {Error = ErrorCode.ERR_SocketDisconnected});
            }

            this.channel.Dispose();
            this.Network.Remove(id);
            this.requestCallback.Clear();
        }

        public IPEndPoint RemoteAddress {
            get { return this.channel.RemoteAddress; }
        }

        public ChannelType ChannelType {
            get { return this.channel.ChannelType; }
        }

        private async void StartRecv(){
            while (true) {
                if (this.IsDisposed) {
                    return;
                }

                Packet packet;
                try {
                    packet = await this.channel.Recv();

                    if (this.IsDisposed) {
                        return;
                    }
                }
                catch (Exception e) {
                    Log.Error(e.ToString());
                    continue;
                }

                try {
                    this.Run(packet);
                }
                catch (Exception e) {
                    Log.Error(e.ToString());
                }
            }
        }

        private void Run(Packet packet){
            if (packet.Length < Packet.MinSize) {
                Log.Error($"message error length < {Packet.MinSize}, ip: {this.RemoteAddress}");
                this.Network.Remove(this.Id);
                return;
            }

            byte flag = packet.Flag();
            ushort opcode = packet.Opcode();

#if !SERVER
            if (OpcodeHelper.IsClientHotfixMessage(opcode)) {
                this.Network.MessageDispatcher.Dispatch(this, packet);
                return;
            }
#endif

            // flag第一位为1表示这是rpc返回消息,否则交由MessageDispatcher分发
            if ((flag & 0x01) == 0) {
                this.Network.MessageDispatcher.Dispatch(this, packet);
                return;
            }

            object message =
                this.Network.MessagePacker.DeserializeFrom(opcode, packet.Bytes, Packet.Index,
                    packet.Length - Packet.Index);

            IResponse response = message as IResponse;
            if (response == null) {
                throw new Exception($"flag is response, but message is not! {opcode}");
            }

            Action<IResponse> action;
            if (!this.requestCallback.TryGetValue(response.RpcId, out action)) {
                return;
            }

            this.requestCallback.Remove(response.RpcId);
            action(response);
        }

        public Task<IResponse> Call(IRequest request){
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) => {
                try {
                    if (response.Error > ErrorCode.ERR_Exception) {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e) {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };

            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

        public Task<IResponse> Call(IRequest request, CancellationToken cancellationToken){
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) => {
                try {
                    if (response.Error > ErrorCode.ERR_Exception) {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e) {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };

            cancellationToken.Register(() => this.requestCallback.Remove(rpcId));

            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

        public void Send(IMessage message){
            this.Send(0x00, message);
        }

        public void Reply(IResponse message){
            if (this.IsDisposed) {
                throw new Exception("session已经被Dispose了");
            }

            this.Send(0x01, message);
        }

        public void Send(byte flag, IMessage message){
            byte[] bytes = this.Network.MessagePacker.SerializeToByteArray(message);
            Send(flag, message.opcode, bytes);
        }

        public void Send(ushort opcode, byte[] bytes){
            Send(0x00,opcode,bytes);
        }

        public void Send(byte flag, ushort opcode, byte[] bytes){
            if (this.IsDisposed) {
                throw new Exception("session已经被Dispose了");
            }

            this.byteses[0][0] = flag;
            this.byteses[1] = BytesHelper.GetBytes(opcode);
            this.byteses[2] = bytes;

#if SERVER
			// 如果是allserver，内部消息不走网络，直接转给session,方便调试时看到整体堆栈
			if (this.Network.AppType == AppType.AllServer)
			{
				Session session = this.Network.Entity.GetComponent<NetInnerComponent>().Get(this.RemoteAddress);
				this.pkt.Length = 0;
				ushort index = 0;
				foreach (var byts in byteses)
				{
					Array.Copy(byts, 0, this.pkt.Bytes, index, byts.Length);
					index += (ushort)byts.Length;
				}

				this.pkt.Length = index;
				session.Run(this.pkt);
				return;
			}
#endif

            channel.Send(this.byteses);
        }

#if SERVER
		private Packet pkt = new Packet(ushort.MaxValue);
#endif
    }
}