using System.Runtime.InteropServices;
using Lockstep.Util;

namespace Lockstep.BehaviourTree {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionLeaf : IBTContent {
        internal int status;
        internal bool needExit;
    }
}