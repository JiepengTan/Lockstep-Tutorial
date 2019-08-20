#pragma warning disable 0169
using System;
using System.Runtime.InteropServices;
using Lockstep.Util;

namespace Lockstep.BehaviourTree {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionParallel:IBTContent {
        public struct BTCParallelStatusEval {
            public const int Size = 16;
            public fixed bool status[Size];

            public void Init(bool val){
                for (int i = 0; i < Size; i++) {
                    status[i] = val;
                }
            }

            void CheckIdx(Int32 index){
                if (index < 0 || index >= Size) {
                    NativeHelper.ArrayOutOfRange();
                }
            }

            public bool this[int index] {
                get {
                    CheckIdx(index);
                    return status[index] ;
                }
                set {
                    CheckIdx(index);
                    status[index] = value;
                }
            }
        }

        public unsafe struct BTCParallelStatusRunning {
            public const int Size = 16;
            public fixed byte status[Size];

            public void Init(byte val){
                for (int i = 0; i < Size; i++) {
                    status[i] = val;
                }
            }

            void CheckIdx(Int32 index){
                if (index < 0 || index >= Size) {
                    NativeHelper.ArrayOutOfRange();
                }
            }

            public byte this[int index] {
                get {
                    CheckIdx(index);
                    return status[index];
                }
                set {
                    CheckIdx(index);
                    status[index] = value;
                }
            }
        }

        internal BTCParallelStatusEval evaluationStatus;
        internal BTCParallelStatusRunning StatusRunning;
    }
}