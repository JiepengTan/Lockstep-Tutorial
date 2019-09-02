#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace Lockstep.CodeGenerator {
    [CreateAssetMenu(fileName = "CodeGenConfig")]
    public class CodeGenConfig : ScriptableObject {
        public string relPath ;
        public string args;
    }
}
#endif    