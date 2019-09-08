using Lockstep.Network;
using Lockstep.Serialization;
using NetMsg.Common;

namespace Lockstep.Game{
    public class MessagePacker : IMessagePacker {
        public static MessagePacker Instance { get; } = new MessagePacker();

        public object DeserializeFrom(ushort opcode, byte[] bytes, int index, int count){
            var type = (EMsgSC) opcode;
            switch (type) {
                //ping 
                case EMsgSC.C2G_PlayerPing: return BaseFormater.FromBytes<Msg_C2G_PlayerPing>(bytes, index, count);
                case EMsgSC.G2C_PlayerPing: return BaseFormater.FromBytes<Msg_G2C_PlayerPing>(bytes, index, count);
                //login
                case EMsgSC.L2C_JoinRoomResult: return BaseFormater.FromBytes<Msg_L2C_JoinRoomResult>(bytes, index, count);
                case EMsgSC.C2L_JoinRoom: return BaseFormater.FromBytes<Msg_C2L_JoinRoom>(bytes, index, count);
                case EMsgSC.C2L_LeaveRoom: return BaseFormater.FromBytes<Msg_C2L_LeaveRoom>(bytes, index, count);
                case EMsgSC.C2G_LoadingProgress: return BaseFormater.FromBytes<Msg_C2G_LoadingProgress>(bytes, index, count);

                //room
                case EMsgSC.G2C_Hello: return BaseFormater.FromBytes<Msg_G2C_Hello>(bytes, index, count);
                case EMsgSC.G2C_FrameData: return BaseFormater.FromBytes<Msg_ServerFrames>(bytes, index, count);
                case EMsgSC.G2C_RepMissFrame: return BaseFormater.FromBytes<Msg_RepMissFrame>(bytes, index, count);
                case EMsgSC.G2C_GameEvent: return BaseFormater.FromBytes<Msg_G2C_GameEvent>(bytes, index, count);
                case EMsgSC.G2C_GameStartInfo: return BaseFormater.FromBytes<Msg_G2C_GameStartInfo>(bytes, index, count);
                case EMsgSC.G2C_LoadingProgress: return BaseFormater.FromBytes<Msg_G2C_LoadingProgress>(bytes, index, count);
                case EMsgSC.G2C_AllFinishedLoaded: return BaseFormater.FromBytes<Msg_G2C_AllFinishedLoaded>(bytes, index, count);

                case EMsgSC.C2G_PlayerInput: return BaseFormater.FromBytes<Msg_PlayerInput>(bytes, index, count);
                case EMsgSC.C2G_ReqMissFrame: return BaseFormater.FromBytes<Msg_ReqMissFrame>(bytes, index, count);
                case EMsgSC.C2G_RepMissFrameAck: return BaseFormater.FromBytes<Msg_RepMissFrameAck>(bytes, index, count);
                case EMsgSC.C2G_HashCode: return BaseFormater.FromBytes<Msg_HashCode>(bytes, index, count);
              }

            return null;
        }

        public byte[] SerializeToByteArray(IMessage msg){
            return ((BaseFormater) msg).ToBytes();
        }
    }
}