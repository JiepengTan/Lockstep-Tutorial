using System;
using System.Collections.Generic;
using System.IO;
using Lockstep.Serialization;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.BehaviourTree {
    [ExtFormat]
    public partial class BaseNodeData :BaseFormater {
        [Index(0)]
        public string AnnotationStr;
        [NoToBytes]
        public bool IsSpread = true;
        [NoToBytes]
        public bool IsCondition = false;
        
        public override void Serialize(Serializer writer){
            var idx = writer.Position;
            writer.Write(0);
            writer.Write(AnnotationStr);
        }

        public override void Deserialize(Deserializer reader){
            AnnotationStr = reader.ReadString();
        }
    }

    public partial class BTInfo : BaseFormater {
        public BTAction RootNode;
        public int[] Offsets;
        public int MemSize;
        public string Description;
        public int Id;

        public BTInfo GetClone(FuncCreateNode funcCreate){
            var bytes = ToBytes();
            return BTInfo.Deserialize(bytes, funcCreate);
        }

        public void Init(){
            int curIdx = 0;
            RootNode.InitNodeId(ref curIdx);
            Offsets = RootNode.GetTotalOffsets();
            MemSize = RootNode.GetTotalMemSize();
        }

    }

    public delegate BTNode FuncCreateNode(short id);

    public partial class BTInfo {
        void SerializeNode(Serializer writer, BTNode node){
            writer.Write(node.NodeId);
            node.Serialize(writer);
            writer.Write(node.Precondition != null);
            if (node.Precondition != null) {
                SerializeNode(writer, node.Precondition);
            }

            writer.Write((byte) node.GetChildCount());
            for (int i = 0; i < node.GetChildCount(); i++) {
                var cnode = node.GetChild(i);
                SerializeNode(writer, cnode);
            }
            
        }

        BTNode DeserializeNode(Deserializer reader, FuncCreateNode funcCreate){
            var id = reader.ReadInt16();
            var node = funcCreate(id);
            if(node == null) Debug.LogError(id.ToString());
            node.Deserialize(reader);
            var hasPrecondition = reader.ReadBoolean();
            if (hasPrecondition) {
                var pNode = DeserializeNode(reader, funcCreate);
                node.Precondition = pNode as BTPrecondition;
            }

            var childCount = reader.ReadByte();
            for (int i = 0; i < childCount; i++) {
                var cnode = DeserializeNode(reader, funcCreate);
                node.AddChild(cnode);
            }

            return node;
        }

        public override void Serialize(Serializer writer){
            Serialize(this, writer);
        }

        public static void Serialize(BTInfo info, Serializer writer){
            writer.Write(info.Id);
            writer.Write(info.Description);
            info.SerializeNode(writer, info.RootNode);
        }
        public static BTInfo Deserialize(byte[] bytes, FuncCreateNode funcCreate){
            var reader = new Deserializer(bytes);
            return Deserialize(reader, funcCreate);
        }
        public static BTInfo Deserialize(Deserializer reader, FuncCreateNode funcCreate){
            var info = new BTInfo();
            info.Id = reader.ReadInt32();
            info.Description = reader.ReadString();
            info.RootNode = info.DeserializeNode(reader, funcCreate) as BTAction;
            info.Init();
            return info;
        }
    }
}