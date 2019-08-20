using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep.BehaviourTree;
using Lockstep.Logging;

namespace Lockstep.BehaviourTree{
    public partial class BTFactory {
        public static string FileExt = ".bytes";
        public const short MaxBuildInIdx = (short)(int)EBTBuildInTypeIdx.MaxEnumCount;
        private static int _curIdx = 0;
        public static BTInfo CreateBtInfo(BTAction bt){
            var offsets = bt.GetTotalOffsets();
            var memSize = bt.GetTotalMemSize();
            return new BTInfo() {
                MemSize = memSize,
                Offsets = offsets,
                RootNode = bt,
            };
        }
        public static void BeforeCreateNode(){
            _curIdx = 0;
        }
        public static T CreateNode<T>() where T : BTNode, new(){
            return new T() {UniqueKey = _curIdx++};
        }

        public static Dictionary<Type, short> type2Id = new Dictionary<Type, short>();
        public static Dictionary<Type, short> gameType2Id = new Dictionary<Type, short>();

        public static  void LogBuildInTypes(){
            var types = typeof(BTFactory).Assembly.GetTypes().Where(t => typeof(BTNode).IsAssignableFrom(t)
                                                                 && !t.IsAbstract 
                                                                 //&& t.Name.StartsWith("BT")
                                                                 ) ;
            StringBuilder sb = new StringBuilder();
            int idx = -1;
            foreach (var type in types) {
                sb.AppendLine($"{{ typeof({type.Name}),{idx--.ToString()} }},");
            }

            Debug.LogError(sb.ToString());
            return;
        }
    }
}