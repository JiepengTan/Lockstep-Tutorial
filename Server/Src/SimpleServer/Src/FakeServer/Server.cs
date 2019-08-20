using System;
using System.Collections.Generic;
using System.Net;
using Lockstep.Logging;
using Lockstep.Network;
using Lockstep.Util;

namespace Lockstep.FakeServer{
    public class Server : IMessageDispatcher {
        public static IPEndPoint serverIpPoint = NetworkUtil.ToIPEndPoint("127.0.0.1", 10083);

        public NetOuterProxy net = new NetOuterProxy();
        private int count = 0;

        public void Start(){
            net.MessageDispatcher = this;
            net.MessagePacker = MessagePacker.Instance;
            net.Awake(NetworkProtocol.TCP, serverIpPoint);
            startUpTimeStamp = lastUpdateTimeStamp = DateTime.Now;
        }

        public void Dispatch(Session session, Packet packet){
            ushort opcode = packet.Opcode();
            var message = session.Network.MessagePacker.DeserializeFrom(opcode, packet.Bytes, Packet.Index,
                packet.Length - Packet.Index) as IMessage;
            //var msg = JsonUtil.ToJson(message);
            //Log.sLog("Server " + msg);
            var type = (EMsgType) opcode;
            switch (type) {
                case EMsgType.JoinRoom:
                    OnPlayerConnect(session, message);
                    break;
                case EMsgType.QuitRoom:
                    OnPlayerQuit(session, message);
                    break;
                case EMsgType.PlayerInput:
                    OnPlayerInput(session, message);
                    break;
                case EMsgType.HashCode:
                    OnPlayerHashCode(session, message);
                    break;
            }
        }

        private DateTime lastUpdateTimeStamp;
        private DateTime startUpTimeStamp;
        private const double updateInterval = 0.015; //frame rate = 30

        private double deltaTime;
        private double timeSinceStartUp;

        public void Update(){
            var now = DateTime.Now;
            deltaTime = (now - lastUpdateTimeStamp).TotalSeconds;
            if (deltaTime > updateInterval) {
                lastUpdateTimeStamp = now;
                timeSinceStartUp = (now - startUpTimeStamp).TotalSeconds;
                DoUpdate();
            }
        }

        public void DoUpdate(){
            //check frame inputs
            var fDeltaTime = (float) deltaTime;
            var fTimeSinceStartUp = (float) timeSinceStartUp;
            room?.DoUpdate(fTimeSinceStartUp, fDeltaTime);
        }

        private Room room;

        public Dictionary<int, PlayerServerInfo> id2Player = new Dictionary<int, PlayerServerInfo>();
        public Dictionary<int, Session> id2Session = new Dictionary<int, Session>();
        public Dictionary<string, PlayerServerInfo> name2Player = new Dictionary<string, PlayerServerInfo>();


        public static int idCounter = 0;

        public int curCount = 0;

        void OnPlayerConnect(Session session, IMessage message){
            //TODO load from db
            
            var msg = message as Msg_JoinRoom;
            var name = msg.name;
            if (name2Player.TryGetValue(name, out var val)) {
                return;
            }

            var info = new PlayerServerInfo();
            info.Id = idCounter++;
            info.name = name;
            name2Player[name] = info;
            id2Player[info.Id] = info;
            id2Session[info.Id] = session;
            session.BindInfo = info;
            curCount++;
            if (curCount >= Room.maxPlayerCount) {
                room = new Room();
                room.Init(0);
                foreach (var player in id2Player.Values) {
                    room.Join(id2Session[player.Id], player);
                }

                OnGameStart(room);
            }
            Debug.Log("OnPlayerConnect count:" + curCount + " " + JsonUtil.ToJson(msg));
        }

        void OnPlayerQuit(Session session, IMessage message){
            Debug.Log("OnPlayerQuit count:" + curCount);
            var player = session.GetBindInfo<PlayerServerInfo>();
            if (player == null)
                return;
            id2Player.Remove(player.Id);
            name2Player.Remove(player.name);
            id2Session.Remove(player.Id);
            curCount--;
            if (curCount == 0) { 
                room = null;
            }
        }

        void OnPlayerInput(Session session, IMessage message){
            var msg = message as Msg_PlayerInput;
            var player = session.GetBindInfo<PlayerServerInfo>();
            room?.OnPlayerInput(player.Id, msg);
        }
        void OnPlayerHashCode(Session session, IMessage message){
            var msg = message as Msg_HashCode;
            var player = session.GetBindInfo<PlayerServerInfo>();
            room?.OnPlayerHashCode(player.Id, msg);
        }

        void OnGameStart(Room room){
            if (room.isRuning) {
                return;
            }

            room.StartGame();
        }
    }
}