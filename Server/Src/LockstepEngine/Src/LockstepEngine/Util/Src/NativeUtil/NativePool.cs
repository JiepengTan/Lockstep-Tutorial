using System;
using System.Collections.Generic;
using System.Linq;

namespace Lockstep.Util {
    public unsafe class NativeFactory {
        static Dictionary<int, NativePool> pools = new Dictionary<int, NativePool>();

        public static long MemSize => pools.Sum((a) => a.Value.MemSize);

        public static NativePool GetPool(int size,bool isAutoPtr = true){
            if (pools.TryGetValue(size, out var pool)) {
                return pool;
            }

            var tPool = new NativePool(size,isAutoPtr);
            pools[size] = tPool;
            return tPool;
        }

        public static void Clear(){
            foreach (var pair in pools) {
                pair.Value.Clear();
            }
        }
    }

    public unsafe class NativePool {
        private List<long> _autoRefPtrs = new List<long>();
        private bool _isAutoPtr;
        private Stack<long> _allPtrs = new Stack<long>();
        private int _typeSize = -1;
        public int MemSize => _typeSize * _allPtrs.Count;


        public NativePool(int typeSize, bool isAutoPtr = true){
            this._typeSize = typeSize;
            this._isAutoPtr = isAutoPtr;
            if (isAutoPtr) {
                _autoRefPtrs = new List<long>();
            }
        }

        public void Return(void* ptr){
            if (ptr == null) NativeHelper.NullPointer();
            _allPtrs.Push((long) ptr);
        }

        public void* Get(){
            if (_allPtrs.Count == 0) return null;
            var ptr = (byte*) _allPtrs.Pop();
            NativeHelper.Zero(ptr, _typeSize);
            return ptr;
        }

        public void* ForceGet(){
            var ptr = Get();
            if (ptr == null) {
                ptr = NativeHelper.AllocAndZero(_typeSize);
                if (_isAutoPtr) {
                    _autoRefPtrs.Add((long) ptr);
                }
            }

            return ptr;
        }

        public void Clear(){
            if (_isAutoPtr) {
                foreach (var ptr in _autoRefPtrs) {
                    NativeHelper.Free((IntPtr) ptr);
                }
                _autoRefPtrs.Clear();
                _allPtrs.Clear();
            }
            else {
                while (_allPtrs.Count > 0) {
                    var ptr = _allPtrs.Pop();
                    NativeHelper.Free((IntPtr) ptr);
                }
            }
        }
    }
}