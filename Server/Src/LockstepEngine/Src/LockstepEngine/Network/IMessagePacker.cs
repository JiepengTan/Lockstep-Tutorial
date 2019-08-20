namespace Lockstep.Network {
    public interface IMessagePacker {
        object DeserializeFrom(ushort type, byte[] bytes, int index, int count);
        byte[] SerializeToByteArray(IMessage msg);
    }
}