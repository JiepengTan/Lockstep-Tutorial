using Lockstep.Network;
using Lockstep.Serialization;

namespace Lockstep.FakeServer{
    public class MessagePacker : IMessagePacker {
        public static MessagePacker Instance { get; } = new MessagePacker();

        public object DeserializeFrom(ushort opcode, byte[] bytes, int index, int count){
            var type = (EMsgType) opcode;
            switch (type) {
                case EMsgType.JoinRoom: return BaseFormater.FromBytes<Msg_JoinRoom>(bytes, index, count);
                case EMsgType.QuitRoom: return BaseFormater.FromBytes<Msg_QuitRoom>(bytes, index, count);
                case EMsgType.PlayerInput: return BaseFormater.FromBytes<Msg_PlayerInput>(bytes, index, count);
                case EMsgType.FrameInput: return BaseFormater.FromBytes<Msg_FrameInput>(bytes, index, count);
                case EMsgType.StartGame: return BaseFormater.FromBytes<Msg_StartGame>(bytes, index, count);
                case EMsgType.HashCode: return BaseFormater.FromBytes<Msg_HashCode>(bytes, index, count);
            }

            return null;
        }

        public byte[] SerializeToByteArray(IMessage msg){
            return ((BaseFormater) msg).ToBytes();
        }
    }
}