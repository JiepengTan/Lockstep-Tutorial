using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Util;

namespace Lockstep.BehaviourTree {
    public unsafe partial class BTWorkingData : TAny {
        ~BTWorkingData(){
            if (_pDatas != null) {
                NativeHelper.Free(new IntPtr(_pDatas));
                _pDatas = null;
            }
        }

        public byte* _pDatas = null;
        private int[] _dataOffset;
        public bool[] HasVisitedInfos;
        public int _dataLen = 0;

        public void ClearRunTimeInfo(){
#if DEBUG
            for (int i = 0; i < HasVisitedInfos.Length; i++) {
                HasVisitedInfos[i] = false;
            }
#endif
        }

        public unsafe void* GetContext(int idx){
            var offset = _dataOffset[idx];
            Debug.Assert(offset >= 0 && offset < _dataLen, " out of range");
            return _pDatas + offset;
        }

        public void Init(int[] offsets, int totalMemSize){
            _pDatas = NativeHelper.AllocAndZero(totalMemSize);
            _dataOffset = offsets;
            HasVisitedInfos = new bool[offsets.Length];
            _dataLen = totalMemSize;
        }

        public BTWorkingData Clone(){
            var ret = new BTWorkingData();
            ret.Init(this._dataOffset, this._dataLen);
            NativeHelper.Copy(this._pDatas, ret._pDatas, _dataLen);
            return ret;
        }
    }
}