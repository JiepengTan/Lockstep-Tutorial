using System.Collections.Generic;
using NetMsg.Common;
using Lockstep.Math;
using Lockstep.Util;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public partial class NetworkService : BaseService, INetworkService {
        public static NetworkService Instance { get; private set; }
        public NetworkService(){
            Instance = this;
        }

        public string ServerIp = "127.0.0.1";
        public int ServerPort = 10083;

        private long _playerID;
        private int _roomId;

        public int Ping => 50; //=> _netProxyRoom.IsInit ? _netProxyRoom.Ping : _netProxyLobby.Ping;
        public bool IsConnected = true; // => _netProxyLobby != null && _netProxyLobby.Connected;

        private bool _noNetwork;
        private bool _isReconnected = false; //是否是重连

        private RoomMsgManager _roomMsgMgr;
        public NetworkMsgHandler _msgHandler = new NetworkMsgHandler();
        public override void DoAwake(IServiceContainer services){
            _noNetwork = _constStateService.IsVideoMode || _constStateService.IsClientMode;
            if (_noNetwork) return;
            _roomMsgMgr = new RoomMsgManager();
            _msgHandler = new NetworkMsgHandler();
            _roomMsgMgr.Init(_msgHandler);
        }

        public override void DoStart(){
            if (_noNetwork) return;
            _roomMsgMgr.ConnectToGameServer(new Msg_C2G_Hello(), null,false);
            //Utils.StartServices();
        }

        public void DoUpdate(LFloat deltaTime){
            if (_noNetwork) return;
            //Utils.UpdateServices();
            _roomMsgMgr?.DoUpdate(deltaTime);
        }


        public override void DoDestroy(){
            if (_noNetwork) return;
            _roomMsgMgr?.DoDestroy();
            _roomMsgMgr = null;
        }


        public void OnEvent_TryLogin(object param){
            if (_noNetwork) return;
            Debug.Log("OnEvent_TryLogin" + param.ToJson());
            //var loginInfo = param as LoginParam;
            //var _account = loginInfo.account;
            //var _password = loginInfo.password;
            //_loginMgr.Login(_account, _password);
        }

        private void OnEvent_OnConnectToGameServer(object param){
            if (_noNetwork) return;
            var isReconnect = (bool) param;
            _constStateService.IsReconnecting = isReconnect;
        }

        private void OnEvent_LevelLoadProgress(object param){
            if (_noNetwork) return;
            _roomMsgMgr.OnLevelLoadProgress((float) param);
            CheckLoadingProgress();
        }

        private void OnEvent_PursueFrameProcess(object param){
            if (_noNetwork) return;
            _roomMsgMgr.FramePursueRate = (float) param;
            CheckLoadingProgress();
        }

        private void OnEvent_PursueFrameDone(object param){
            if (_noNetwork) return;
            _roomMsgMgr.FramePursueRate = 1;
            CheckLoadingProgress();
        }

        void CheckLoadingProgress(){
            if (_roomMsgMgr.IsReconnecting) {
                var curProgress = _roomMsgMgr.CurProgress / 100.0f;
                EventHelper.Trigger(EEvent.ReconnectLoadProgress, curProgress);
                if (_roomMsgMgr.CurProgress >= 100) {
                    _constStateService.IsReconnecting = false;
                    _roomMsgMgr.IsReconnecting = false;
                    EventHelper.Trigger(EEvent.ReconnectLoadDone);
                }
            }
        }

        #region Login Handler

        public void CreateRoom(int mapId, string name, int size){
            //_loginMgr.CreateRoom(mapId, name, size);
        }

        public void StartGame(){
            //_loginMgr.StartGame();
        }

        public void ReadyInRoom(bool isReady){
            //_loginMgr.ReadyInRoom(isReady);
        }

        public void JoinRoom(int roomId){
            //_loginMgr.JoinRoom(roomId, (infos) => { EventHelper.Trigger(EEvent.OnJoinRoomResult, infos); });
        }

        public void ReqRoomList(int startIdx){
            //_loginMgr.ReqRoomList(startIdx);
        }

        public void LeaveRoom(){
            //_loginMgr.LeaveRoom();
        }


        public void SendChatInfo(RoomChatInfo chatInfo){
            //_loginMgr.SendChatInfo(chatInfo);
        }

        #endregion

        #region Room Msg Handler

        public void SendGameEvent(byte[] data){
            if (_noNetwork) return;
            _roomMsgMgr.SendGameEvent(data);
        }

        public void SendPing(byte localId, long timestamp){
            if (_noNetwork) return;
            _roomMsgMgr.SendPing(localId,timestamp);
        }

        public void SendInput(Msg_PlayerInput msg){
            if (_noNetwork) return;
            _roomMsgMgr.SendInput(msg);
        }

        public void SendMissFrameReq(int missFrameTick){
            if (_noNetwork) return;
            _roomMsgMgr.SendMissFrameReq(missFrameTick);
        }

        public void SendMissFrameRepAck(int missFrameTick){
            if (_noNetwork) return;
            _roomMsgMgr.SendMissFrameRepAck(missFrameTick);
        }

        public void SendHashCodes(int firstHashTick, List<int> allHashCodes, int startIdx, int count){
            if (_noNetwork) return;
            _roomMsgMgr.SendHashCodes(firstHashTick, allHashCodes, startIdx, count);
        }

        #endregion
    }
}