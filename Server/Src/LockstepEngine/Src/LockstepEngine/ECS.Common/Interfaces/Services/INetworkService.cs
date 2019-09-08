using System.Collections.Generic;
using Lockstep.Serialization;
using NetMsg.Common;

namespace Lockstep.Game {
    public interface INetworkService :IService {
        void SendGameEvent(byte[] data);
        void SendPing(byte localId,long timestamp);
        void SendInput(Msg_PlayerInput msg);
        void SendHashCodes(int startTick,List<int> hashCodes,int startIdx,int count);
        void SendMissFrameReq(int missFrameTick);
        void SendMissFrameRepAck(int missFrameTick);
    }
}