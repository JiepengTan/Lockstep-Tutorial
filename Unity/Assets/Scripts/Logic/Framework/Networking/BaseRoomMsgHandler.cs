using Lockstep.Logging;
using NetMsg.Common;

namespace Lockstep.Game {
    public interface IRoomMsgHandler {
        void SetLogger(DebugInstance debug);
        void OnTcpHello(Msg_G2C_Hello msg);
        void OnUdpHello(int mapId, byte localId);
        void OnGameStartInfo(Msg_G2C_GameStartInfo data);
        void OnLoadingProgress(byte[] progresses);
        void OnAllFinishedLoaded(short level);
        void OnGameStartFailed();
        void OnServerFrames(Msg_ServerFrames msg);
        void OnMissFrames(Msg_RepMissFrame msg);
        void OnGameEvent(byte[] data);
    }

    public class BaseRoomMsgHandler : BaseLogger, IRoomMsgHandler {
        public virtual void OnTcpHello(Msg_G2C_Hello msg){ }
        public virtual void OnUdpHello(int mapId, byte localId){ }
        public virtual void OnGameStartInfo(Msg_G2C_GameStartInfo data){ }
        public virtual void OnLoadingProgress(byte[] progresses){ }
        public virtual void OnAllFinishedLoaded(short level){ }
        public virtual void OnGameStartFailed(){ }
        public virtual void OnServerFrames(Msg_ServerFrames msg){ }
        public virtual void OnMissFrames(Msg_RepMissFrame msg){ }
        public virtual void OnGameEvent(byte[] data){ }
    }
}