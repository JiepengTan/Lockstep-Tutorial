using System.Runtime.InteropServices;
using Lockstep.Util;

namespace Lockstep.BehaviourTree {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionNonPrioritizedSelector :IBTContent{
        public int currentSelectedIndex;
        public int lastSelectedIndex;

        public BTCActionNonPrioritizedSelector(int curIdx = -1, int lastIdx = -1){
            currentSelectedIndex = curIdx;
            lastSelectedIndex = lastIdx;
        }
    }
}