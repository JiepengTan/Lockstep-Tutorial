using System;
using System.Runtime.InteropServices;

namespace Lockstep.Serialization
{
    public static class FastBitConverter
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterHelperDouble
        {
            [FieldOffset(0)]
            public ulong Along;

            [FieldOffset(0)]
            public double Adouble;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterHelperFloat
        {
            [FieldOffset(0)]
            public int Aint;

            [FieldOffset(0)]
            public float Afloat;
        }

        private static void WriteLittleEndian(byte[] buffer, int offset, ulong data)
        {
#if BIGENDIAN
            buffer[offset + 7] = (byte)(data);
            buffer[offset + 6] = (byte)(data >> 8);
            buffer[offset + 5] = (byte)(data >> 16);
            buffer[offset + 4] = (byte)(data >> 24);
            buffer[offset + 3] = (byte)(data >> 32);
            buffer[offset + 2] = (byte)(data >> 40);
            buffer[offset + 1] = (byte)(data >> 48);
            buffer[offset    ] = (byte)(data >> 56);
#else
            buffer[offset] = (byte)(data);
            buffer[offset + 1] = (byte)(data >> 8);
            buffer[offset + 2] = (byte)(data >> 16);
            buffer[offset + 3] = (byte)(data >> 24);
            buffer[offset + 4] = (byte)(data >> 32);
            buffer[offset + 5] = (byte)(data >> 40);
            buffer[offset + 6] = (byte)(data >> 48);
            buffer[offset + 7] = (byte)(data >> 56);
#endif
        }

        private static void WriteLittleEndian(byte[] buffer, int offset, int data)
        {
#if BIGENDIAN
            buffer[offset + 3] = (byte)(data);
            buffer[offset + 2] = (byte)(data >> 8);
            buffer[offset + 1] = (byte)(data >> 16);
            buffer[offset    ] = (byte)(data >> 24);
#else
            buffer[offset] =    (byte)(data);
            buffer[offset + 1] = (byte)(data >> 8);
            buffer[offset + 2] = (byte)(data >> 16);
            buffer[offset + 3] = (byte)(data >> 24);
#endif
        }

        public static void WriteLittleEndian(byte[] buffer, int offset, short data)
        {
#if BIGENDIAN
            buffer[offset + 1] = (byte)(data);
            buffer[offset    ] = (byte)(data >> 8);
#else
            buffer[offset] = (byte)(data);
            buffer[offset + 1] = (byte)(data >> 8);
#endif
        }

        public static void GetBytes(byte[] bytes, int startIndex, double value)
        {
            ConverterHelperDouble ch = new ConverterHelperDouble { Adouble = value };
            WriteLittleEndian(bytes, startIndex, ch.Along);
        }

        public static void GetBytes(byte[] bytes, int startIndex, float value)
        {
            ConverterHelperFloat ch = new ConverterHelperFloat { Afloat = value };
            WriteLittleEndian(bytes, startIndex, ch.Aint);
        }

        public static void GetBytes(byte[] bytes, int startIndex, bool value)
        {
            bytes[startIndex] = (byte)(value?1:0);
        }
        public static void GetBytes(byte[] bytes, int startIndex, short value)
        {
            WriteLittleEndian(bytes, startIndex, value);
        }
        public static void GetBytes(byte[] bytes, int startIndex, ushort value)
        {
            WriteLittleEndian(bytes, startIndex, (short)value);
        }

        public static void GetBytes(byte[] bytes, int startIndex, int value)
        {
            WriteLittleEndian(bytes, startIndex, value);
        }

        public static void GetBytes(byte[] bytes, int startIndex, uint value)
        {
            WriteLittleEndian(bytes, startIndex, (int)value);
        }

        public static void GetBytes(byte[] bytes, int startIndex, long value)
        {
            WriteLittleEndian(bytes, startIndex, (ulong)value);
        }

        public static void GetBytes(byte[] bytes, int startIndex, ulong value)
        {
            WriteLittleEndian(bytes, startIndex, value);
        }
        
        
        
        private static ulong ReadLittleEndian(byte[] buffer,int offset, int count)
        {
            ulong r = 0;
            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < count; i++)
                {
                    r |= (ulong)buffer[offset + i] << i * 8;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    r |= (ulong)buffer[offset + count - 1 - i] << i * 8;
                }
            }
            return r;
        }
        
        public static short ToInt16(byte[] buffer,int index)
        {
            return (short)ReadLittleEndian(buffer, index, sizeof(short));
        }

        public static ushort ToUInt16(byte[] buffer,int index)
        {
            return (ushort)ReadLittleEndian(buffer, index, sizeof(ushort));
        }

        public static int ToInt32(byte[] buffer,int index)
        {
            return (int)ReadLittleEndian(buffer, index, sizeof(int));
        }

        public static uint ToUInt32(byte[] buffer,int index)
        {
            return (uint)ReadLittleEndian(buffer, index, sizeof(uint));
        }

        public static long ToInt64(byte[] buffer,int index)
        {
            return (long)ReadLittleEndian(buffer, index, sizeof(long));
        }

        public static ulong ToUInt64(byte[] buffer,int index)
        {
            return ReadLittleEndian(buffer, index, sizeof(ulong));
        }
        
        public static float ToSingle(byte[] buffer,int index)
        {
            int i = (int)ReadLittleEndian(buffer, index, sizeof(float));
            ConverterHelperFloat ch = new ConverterHelperFloat { Aint = i };
            return ch.Afloat;
        }

        public static double ToDouble(byte[] buffer,int index)
        {
            ulong i = ReadLittleEndian(buffer, index, sizeof(double));
            ConverterHelperDouble ch = new ConverterHelperDouble { Along = i };
            return ch.Adouble;
        }
    }
}
