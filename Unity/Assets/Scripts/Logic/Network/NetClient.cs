using System;
using System.Net;
using Lockstep.Network;
using Lockstep.Serialization;
using Lockstep.Game;
using NetMsg.Common;

namespace Lockstep.Game{
    public class NetClient : IMessageDispatcher {
        public static IPEndPoint serverIpPoint = NetworkUtil.ToIPEndPoint("127.0.0.1", 10083);
        private NetOuterProxy net = new NetOuterProxy();
        public Session Session;
        public Action<ushort, object> NetMsgHandler;

        private int count = 0;
        public int id;

        public void DoStart(){
            net.Awake(NetworkProtocol.TCP);
            net.MessageDispatcher = this;
            net.MessagePacker = MessagePacker.Instance;
            Session = net.Create(serverIpPoint);
            Session.Start();
        }

        public void DoDestroy(){
            if (Session != null) {
                net.Dispose();
                Session = null;
            }
        }

        public void Dispatch(Session session, Packet packet){
            ushort opcode = packet.Opcode();
            var message = session.Network.MessagePacker.DeserializeFrom(opcode, packet.Bytes, Packet.Index,
                packet.Length - Packet.Index);
            NetMsgHandler?.Invoke(opcode,message);
        }


        public void Send(IMessage msg){
            Session.Send(msg);
        }
        public void SendMessage(EMsgSC type, byte[] bytes){
            Session?.Send(0x00, (ushort) type, bytes);
        }
        
    }
}