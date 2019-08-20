using System;
using System.Diagnostics;
using Lockstep.Serialization;

namespace NetMsg.Common {
    
    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class InputCmd : BaseMsg {
        public byte type => content[0];

        public byte[] content;
        public InputCmd(){ }

        public InputCmd(byte type){
            content = new byte[1] {type};
        }

        public InputCmd(byte[] bytes){
            content = bytes;
        }

        public bool Equals(InputCmd cmdb){
            if (cmdb == null) return false;
            return content.EqualsEx(cmdb.content);
        }

        public override bool Equals(object obj){
            var cmdb = obj as InputCmd;
            return Equals(cmdb);
        }

        public override int GetHashCode(){
            return content.GetHashCode();
        }

        public override string ToString(){
            return $"t:{content[0]} content:{content?.Length ?? 0}";
        }


        public override void Serialize(Serializer writer){
            Debug.Assert(content != null && content.Length>0&& content.Length < byte.MaxValue
                , $"!!!!!!!!! Input Cmd len{content?.Length ?? 0} should less then {byte.MaxValue}");
            writer.WriteBytes_255(content);
        }

        public override void Deserialize(Deserializer reader){
            content = reader.ReadBytes_255();
            Debug.Assert(content != null && content.Length > 0,"!!!!!!!!! Input Cmd len{content?.Length ?? 0} should less then {byte.MaxValue}");
        }
    }
}