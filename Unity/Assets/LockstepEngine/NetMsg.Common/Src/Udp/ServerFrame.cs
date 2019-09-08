using Lockstep.Serialization;

namespace NetMsg.Common {
    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class PlayerPing : BaseMsg {
        public byte localId;
        public long sendTimestamp;
        
        public override void Serialize(Serializer writer){
            writer.Write(localId);
            writer.Write(sendTimestamp);
        }

        public override void Deserialize(Deserializer reader){
            localId = reader.ReadByte();
            sendTimestamp = reader.ReadInt64();
        }
    }

    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_C2G_PlayerPing : PlayerPing { }

    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_G2C_PlayerPing : PlayerPing {
        public long timeSinceServerStart;
        public override void Serialize(Serializer writer){
            base.Serialize(writer);
            writer.Write(timeSinceServerStart);
        }

        public override void Deserialize(Deserializer reader){
            base.Deserialize(reader);
            timeSinceServerStart = reader.ReadInt64();
        }
    }

    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class ServerFrame : BaseMsg {
        public byte[] inputDatas; //包含玩家的输入& 游戏输入
        public int tick;
        public Msg_PlayerInput[] _inputs;

        public Msg_PlayerInput[] Inputs {
            get { return _inputs; }
            set {
                _inputs = value;
                inputDatas = null;
            }
        }

        private byte[] _serverInputs;
        
        public byte[] ServerInputs {//服务器输入 如掉落等
            get { return _serverInputs; }
            set {
                _serverInputs = value;
                inputDatas = null;
            }
        }

        public void BeforeSerialize(){
            if (inputDatas != null) return;
            var writer = new Serializer();
            var inputLen = (byte) (Inputs?.Length ?? 0);
            writer.Write(inputLen);
            for (byte i = 0; i < inputLen; i++) {
                var cmds = Inputs[i].Commands;
                var len = (byte) (cmds?.Length ?? 0);
                writer.Write(len);
                for (int cmdIdx = 0; cmdIdx < len; cmdIdx++) {
                    cmds[cmdIdx].Serialize(writer);
                }
            }

            writer.WriteBytes_255(_serverInputs);
            inputDatas = writer.CopyData();
        }

        public void AfterDeserialize(){
            var reader = new Deserializer(inputDatas);
            var inputLen = reader.ReadByte();
            _inputs = new Msg_PlayerInput[inputLen];
            for (byte i = 0; i < inputLen; i++) {
                var input = new Msg_PlayerInput();
                input.Tick = tick;
                input.ActorId = i;
                _inputs[i] = input;
                var len = reader.ReadByte();
                if (len == 0) {
                    input.Commands = null;
                    continue;
                }

                input.Commands = new InputCmd[len];
                for (int cmdIdx = 0; cmdIdx < len; cmdIdx++) {
                    var cmd = new InputCmd();
                    cmd.Deserialize(reader);
                    input.Commands[cmdIdx] = cmd;
                }
            }

            _serverInputs = reader.ReadBytes_255();
        }

        public override void Serialize(Serializer writer){
            BeforeSerialize();
            writer.Write(tick);
            writer.Write(inputDatas);
        }

        public override void Deserialize(Deserializer reader){
            tick = reader.ReadInt32();
            inputDatas = reader.ReadBytes();
            AfterDeserialize();
        }

        public override string ToString(){
            var count = (inputDatas == null) ? 0 : inputDatas.Length;
            return
                $"t:{tick} " +
                $"inputNum:{count}";
        }

        public override bool Equals(object obj){
            if (obj == null) return false;
            var frame = obj as ServerFrame;
            return Equals(frame);
        }

        public override int GetHashCode(){
            return tick;
        }

        public bool Equals(ServerFrame frame){
            if (frame == null) return false;
            if (tick != frame.tick) return false;
            BeforeSerialize();
            frame.BeforeSerialize();
            return inputDatas.EqualsEx(frame.inputDatas);
        }
    }
}