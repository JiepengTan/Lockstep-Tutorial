using LitJson;
using Lockstep.Logging;
using NetMsg.Common;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public class NetworkMsgHandler : IRoomMsgHandler, ILoginHandler {
        void Log(string msg){
            Debug.Log(msg);
        }

        void ILoginHandler.SetLogger(DebugInstance debug){ }
        void IRoomMsgHandler.SetLogger(DebugInstance debug){ }

        public void OnConnectedLoginServer(){
            EventHelper.Trigger(EEvent.OnConnLogin);
            Log("OnConnLogin ");
        }

        public void OnLoginFailed(ELoginResult result){
            Log("Login failed reason " + result);
            EventHelper.Trigger(EEvent.OnLoginFailed, result);
        }

        public void OnConnLobby(RoomInfo[] roomInfos){
            EventHelper.Trigger(EEvent.OnLoginResult, roomInfos);
        }

        public void OnRoomInfo(RoomInfo[] roomInfos){
            Log("UpdateRoomsState " + (roomInfos == null ? "null" : JsonMapper.ToJson(roomInfos)));
            EventHelper.Trigger(EEvent.OnRoomInfoUpdate, roomInfos);
        }


        public void OnCreateRoom(RoomInfo roomInfo, RoomPlayerInfo[] playerInfos){
            if (roomInfo == null)
                Log("CreateRoom failed reason ");
            else {
                Log("CreateRoom " + roomInfo.ToString());
                EventHelper.Trigger(EEvent.OnCreateRoom, roomInfo);
            }
        }

        public void OnStartRoomResult(int reason){
            if (reason != 0) {
                Log("StartGame failed reason " + reason);
            }
        }

        public void OnGameStart(Msg_C2G_Hello msg, IPEndInfo tcpEnd, bool isReconnect){
            Log("OnGameStart " + msg + " tcpEnd " + tcpEnd);
            EventHelper.Trigger(EEvent.OnConnectToGameServer, isReconnect);
        }

        public void OnPlayerJoinRoom(RoomPlayerInfo info){
            EventHelper.Trigger(EEvent.OnPlayerJoinRoom, info);
        }

        public void OnPlayerLeaveRoom(long userId){
            EventHelper.Trigger(EEvent.OnPlayerLeaveRoom, userId);
        }

        public void OnRoomChatInfo(RoomChatInfo info){
            EventHelper.Trigger(EEvent.OnRoomChatInfo, info);
        }

        public void OnPlayerReadyInRoom(long userId, byte state){
            EventHelper.Trigger(EEvent.OnPlayerReadyInRoom, new object[] {userId, state});
        }

        public void OnLeaveRoom(){
            EventHelper.Trigger(EEvent.OnLeaveRoom);
        }

        public void OnTickPlayer(byte reason){
            EventHelper.Trigger(EEvent.OnTickPlayer, reason);
        }

        public void OnRoomInfoUpdate(RoomInfo[] addInfo, int[] deleteInfos, RoomChangedInfo[] changedInfos){ }

        public void OnTcpHello(Msg_G2C_Hello msg){
            Log($"OnTcpHello msg:{msg} ");
            EventHelper.Trigger(EEvent.OnGameCreate, msg);
            //CoroutineHelper.StartCoroutine(YiledLoadingMap());
        }

        public void OnUdpHello(int mapId, byte localId){
            Log($"OnUdpHello mapId:{mapId} localId:{localId}");
        }

        public void OnGameStartInfo(Msg_G2C_GameStartInfo data){
            Log("Msg_G2C_GameStartInfo ");
            EventHelper.Trigger(EEvent.OnGameStartInfo, data);
        }

        public void OnGameStartFailed(){
            Log($"OnGameStartFailed");
        }

        public void OnLoadingProgress(byte[] progresses){
            EventHelper.Trigger(EEvent.OnLoadingProgress, progresses);
        }

        public void OnAllFinishedLoaded(short level){
            Log("OnAllFinishedLoaded " + level);
            EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, level);
        }

        public void OnServerFrames(Msg_ServerFrames msg){
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public void OnMissFrames(Msg_RepMissFrame msg){
            Log("OnMissFrames");
            if (msg == null) return;
            EventHelper.Trigger(EEvent.OnServerMissFrame, msg);
        }

        public void OnGameEvent(byte[] data){ }
    }
}