#define DEBUG_FRAME_DELAY
using System.Collections.Generic;
using Lockstep.Serialization;

namespace NetMsg.Common {
    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_PlayerInput : BaseMsg {
        public byte[] InputDatas; //real data
        public bool IsMiss;
        public byte ActorId;
        public int Tick;
        public InputCmd[] Commands;
#if DEBUG_FRAME_DELAY
        public float timeSinceStartUp;
#endif

        public Msg_PlayerInput(int tick, byte actorID, List<InputCmd> inputs){
            this.Tick = tick;
            this.ActorId = actorID;
            if (inputs != null && inputs.Count > 0) {
                this.Commands = inputs.ToArray();
            }
        }
        public Msg_PlayerInput(int tick, byte actorID, InputCmd[] inputs = null){
            this.Tick = tick;
            this.ActorId = actorID;
            if (inputs != null && inputs.Length > 0) {
                this.Commands = inputs;
            }
        }
        
        public Msg_PlayerInput(){ }

        public override bool Equals(object obj){
            return Equals(obj as Msg_PlayerInput);
        }

        public bool Equals(Msg_PlayerInput other){
            if (other == null) return false;
            if (Tick != other.Tick) return false;
            return Commands.EqualsEx(other.Commands);
        }

        public override int GetHashCode(){
            return (int) (ActorId << 24 & Tick);
        }

        /// <summary>
        /// TODO     合并 输入
        /// </summary>
        /// <param name="inputb"></param>
        public void Combine(Msg_PlayerInput inputb){ }

        public override void Serialize(Serializer writer){
#if DEBUG_FRAME_DELAY
            writer.Write(timeSinceStartUp);
#endif
            writer.Write(IsMiss);
            writer.Write(ActorId);
            writer.Write(Tick);
            int count = Commands?.Length ?? 0;
            writer.Write((byte) count);
            for (int i = 0; i < count; i++) {
                Commands[i].Serialize(writer);
            }
        }

        public override void Deserialize(Deserializer reader){
#if DEBUG_FRAME_DELAY
            timeSinceStartUp = reader.ReadSingle();
#endif
            IsMiss = reader.ReadBoolean();
            ActorId = reader.ReadByte();
            Tick = reader.ReadInt32();
            int count = reader.ReadByte();
            if (count == 0) {
                Commands = null;
                return;
            }

            Commands = new InputCmd[count];
            for (int i = 0; i < count; i++) {
                var cmd = reader.Parse<InputCmd>();
                Commands[i] = cmd;
            }
        }
    }
}