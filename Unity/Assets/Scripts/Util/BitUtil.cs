namespace Lockstep.Util {
    public static partial class BitUtil {
        public static bool HasBit(int val, int idx){
            return (val & 1 << idx) != 0;
        }

        public static void SetBit(ref int val, int idx, bool isSet){
            if (isSet) {
                val |= (1 << idx);
            }
            else {
                val &= ~(1 << idx);
            }
        }

        public static bool HasBit(byte val, byte idx){
            return (val & 1 << idx) != 0;
        }

        public static void SetBit(ref byte val, byte idx){
            val |= (byte) (1 << idx);
        }

        public static byte ToByte(byte idx){
            return (byte) (1 << idx);
        }
    }
}