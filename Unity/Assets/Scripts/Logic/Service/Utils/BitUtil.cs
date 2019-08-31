namespace Lockstep.Game {
    public static  class EnumBitUtil {
        public static byte ToByte(EInputCmdType idx){
            return (byte) (1 << (byte) idx);
        }

        public static bool HasBit(byte val, EInputCmdType idx){
            return (val & (1 << (byte) idx)) != 0;
        }
    }
}