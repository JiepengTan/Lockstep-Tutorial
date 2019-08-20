using System;

namespace Lockstep.Network {
    public class RpcException : Exception {
        public int Error { get; private set; }

        public RpcException(int error, string message) : base($"Error: {error} Message: {message}"){
            this.Error = error;
        }

        public RpcException(int error, string message, Exception e) : base($"Error: {error} Message: {message}", e){
            this.Error = error;
        }
    }

    public static class ErrorCode {
        public const int ERR_Success = 0;
        public const int ERR_NotFoundActor = 2;

        public const int ERR_AccountOrPasswordError = 102;
        public const int ERR_SessionActorError = 103;
        public const int ERR_NotFoundUnit = 104;
        public const int ERR_ConnectGateKeyError = 105;

        public const int ERR_Exception = 1000;

        public const int ERR_RpcFail = 2001;
        public const int ERR_SocketDisconnected = 2002;
        public const int ERR_ReloadFail = 2003;
        public const int ERR_ActorLocationNotFound = 2004;
    }

    public static class BytesHelper {
        public static byte[] GetBytes(ushort val){
            if (BitConverter.IsLittleEndian) {
                return BitConverter.GetBytes(val);
            }
            else {
                return BitConverter.GetBytes(val).Swap();
            }
        }

        public static byte[] GetBytes(int val){
            if (BitConverter.IsLittleEndian) {
                return BitConverter.GetBytes(val);
            }
            else {
                return BitConverter.GetBytes(val).Swap();
            }
        }

        public static ushort ToUInt16(byte[] value, int startIndex){
            if (BitConverter.IsLittleEndian) {
                return BitConverter.ToUInt16(value, startIndex);
            }
            else {
                return BitConverter.ToUInt16(value.Swap(startIndex, sizeof(ushort)), startIndex);
            }
        }

        private static byte[] Swap(this byte[] vals, int startIdx, int len){
            var dst = new byte[len];
            Buffer.BlockCopy(vals, startIdx, dst, 0, len);
            return Swap(dst);
        }

        private static byte[] Swap(this byte[] vals){
            var count = vals.Length;
            for (int i = 0, j = count - 1; i < j; ++i, --j) {
                var temp = vals[i];
                vals[i] = vals[j];
                vals[j] = temp;
            }

            return vals;
        }
    }
}