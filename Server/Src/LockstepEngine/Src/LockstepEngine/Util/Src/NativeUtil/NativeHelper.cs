using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Lockstep.Util {
    public unsafe class NativeHelper {
        public const int STRUCT_PACK = 4;

#if DEBUG
        public static HashSet<long> _allocedPtrs = new HashSet<long>();
        public static Dictionary<long, int> _prt2Size = new Dictionary<long, int>();
        public static long MemSize => _prt2Size.Sum((d) => d.Value);
#endif
        public static void Free(IntPtr ptr){
            Free(ptr.ToPointer());
        }
        public static void Free(void* ptr){
#if DEBUG
            if (!_allocedPtrs.Contains((long) ptr)) {
                throw new NullReferenceException("Try to free a block which did not allocated!");
            }
            _prt2Size.Remove((long) ptr);
#endif

            if (ptr == null) throw new NullReferenceException();
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        public static IntPtr Alloc(int size){
            var ptr = Marshal.AllocHGlobal(size);
#if DEBUG
            _allocedPtrs.Add((long) ptr);
            _prt2Size.Add((long) ptr, size);
#endif
            return ptr;
        }

        public static void* Resize(void* src, int srcSize, int dstSize){
            var newAry = AllocAndZero(dstSize);
            Copy(newAry, src, srcSize);
            Free(new IntPtr(src));
            return newAry;
        }

        public static unsafe void Zero(void* ptr, int size){
            Zero((byte*) ptr, size);
        }

        public static unsafe void Zero(byte* ptr, int size){
            for (; size >= 4; size -= 4) {
                *(int*) ptr = 0;
                ptr += 4;
            }

            for (; size > 0; --size) {
                *ptr = 0;
            }
        }

        public static unsafe void Copy(void* dest, void* src, int size){
            Copy((byte*) dest, (byte*) src, size);
        }

        public static unsafe void Copy(byte* dest, byte* src, int size){
            for (; size >= 4; size -= 4) {
                *(int*) dest = *(int*) src;
                dest += 4;
                src += 4;
            }

            for (; size > 0; --size) {
                *dest = *src;
                ++dest;
                ++src;
            }
        }

        public static byte* AllocAndZero(int size){
            var ptr = (byte*) (Alloc(size).ToPointer());
            Zero(ptr, size);
            return ptr;
        }

        public static void NullPointer(){
            throw new NullReferenceException("Method invoked on null pointer.");
        }

        public static void ArrayOutOfRange(){
            throw new ArgumentOutOfRangeException("Array index out of range");
        }

        private class BufferSizeMissMatchException : Exception {
            public BufferSizeMissMatchException(string info) : base("Buffer Size miss match!! " + info){ }
        }

        public static void BufferSizeMissMatch(string info = ""){
            throw new BufferSizeMissMatchException(info);
        }
    }
}