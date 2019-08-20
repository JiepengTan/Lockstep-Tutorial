using System.Diagnostics;

namespace Lockstep.Util {
    public class Profiler {
        [Conditional("DEBUG")]
        public static void BeginSample(string tag){ }

        [Conditional("DEBUG")]
        public static void EndSample(){ }
    }
}