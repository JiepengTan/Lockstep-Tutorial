using Lockstep.Serialization;

namespace NetMsg.Common {
    [SelfImplement]
    [Udp]
    public partial class MutilDiscFrames : BaseMsg {
        public int startTick;
        public ServerFrame[] frames;

        public override void Serialize(Serializer writer){
            writer.Write(startTick);
            writer.Write(frames);
        }

        public override void Deserialize(Deserializer reader){
            startTick = reader.ReadInt32();
            frames = reader.ReadArray(frames);
        }
    }
}