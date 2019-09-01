using Lockstep.Logging;
using NetMsg.Common;

namespace Lockstep.Game {
    public interface ILoginHandler {
        void SetLogger(DebugInstance logger);
        void OnTickPlayer(byte reason);
        void OnConnectedLoginServer();
        void OnConnLobby(RoomInfo[] roomInfos);
        void OnRoomInfo(RoomInfo[] roomInfos);
        void OnCreateRoom(RoomInfo info, RoomPlayerInfo[] playerInfos);

        void OnRoomInfoUpdate(RoomInfo[] addInfo, int[] deleteInfos, RoomChangedInfo[] changedInfos);

        void OnStartRoomResult(int reason);
        void OnGameStart(Msg_C2G_Hello msg, IPEndInfo tcpEnd,bool isConnect);
        void OnLoginFailed(ELoginResult result);
        void OnGameStartFailed();
        void OnPlayerJoinRoom(RoomPlayerInfo info);
        void OnPlayerLeaveRoom(long userId);
        void OnRoomChatInfo(RoomChatInfo info);
        void OnPlayerReadyInRoom(long userId, byte state);
        void OnLeaveRoom();
    }

    public class BaseLoginHandler : BaseLogger,ILoginHandler {

        public virtual void OnTickPlayer(byte reason){ }
        public virtual void OnConnectedLoginServer(){ }
        public virtual void OnConnLobby(RoomInfo[] roomInfos){ }
        public virtual void OnRoomInfo(RoomInfo[] roomInfos){ }
        public virtual void OnCreateRoom(RoomInfo info, RoomPlayerInfo[] playerInfos){ }

        public virtual void OnRoomInfoUpdate(RoomInfo[] addInfo, int[] deleteInfos, RoomChangedInfo[] changedInfos){ }

        public virtual void OnStartRoomResult(int reason){ }
        public virtual void OnGameStart(Msg_C2G_Hello msg, IPEndInfo tcpEnd,bool isConnect){ }
        public virtual void OnLoginFailed(ELoginResult result){Log("Login failed reason " + result);}
        public virtual void OnGameStartFailed(){ }
        public virtual void OnPlayerJoinRoom(RoomPlayerInfo info){ }
        public virtual void OnPlayerLeaveRoom(long userId){ }
        public virtual void OnRoomChatInfo(RoomChatInfo info){ }
        public virtual void OnPlayerReadyInRoom(long userId, byte state){ }
        public virtual void OnLeaveRoom(){ }
    }
}