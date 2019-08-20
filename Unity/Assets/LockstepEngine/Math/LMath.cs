
namespace Lockstep.Math
{
    public static partial class LMath
    {
        public static readonly LFloat PIHalf = new LFloat(true,1571);
        public static readonly LFloat PI = new LFloat(true,3142);
        public static readonly LFloat PI2 = new LFloat(true,6283);
        public static readonly LFloat Rad2Deg = 180 / PI;
        public static readonly LFloat Deg2Rad = PI / 180;

        public static LFloat Pi => PI;

        public static LFloat Atan2(LFloat y, LFloat x)
        {
            return Atan2(y._val, x._val);
        }

        public static LFloat Atan2(int y, int x)
        {
            if (x == 0) {
                if (y > 0) return PIHalf;
                else if (y < 0) {return -PIHalf;}
                else return LFloat.zero; 
            }     
            if (y == 0) {
                if (x > 0) return LFloat.zero;
                else if (x < 0) {return PI;}
                else return LFloat.zero; 
            }

            int num;
            int num2;
            if (x < 0)
            {
                if (y < 0)
                {
                    x = -x;
                    y = -y;
                    num = 1;
                }
                else
                {
                    x = -x;
                    num = -1;
                }

                num2 = -31416;
            }
            else
            {
                if (y < 0)
                {
                    y = -y;
                    num = -1;
                }
                else
                {
                    num = 1;
                }

                num2 = 0;
            }

            int dIM = LUTAtan2.DIM;
            long num3 = (long) (dIM - 1);
            long b = (long) ((x >= y) ? x : y);
            int num4 = (int) ((long) x * num3 / b);
            int num5 = (int) ((long) y * num3 / b);
            int num6 = LUTAtan2.table[num5 * dIM + num4];
            return new LFloat(true,(long) ((num6 + num2) * num) / 10);
        }

        public static LFloat Acos(LFloat val)
        {
            int num = (int) (val._val * (long) LUTAcos.HALF_COUNT / LFloat.Precision) +
                      LUTAcos.HALF_COUNT;
            num = Clamp(num, 0, LUTAcos.COUNT);
            return new LFloat(true,(long) LUTAcos.table[num] / 10);
        }
        
        public static LFloat Asin(LFloat val)
        {
            int num = (int) (val._val * (long) LUTAsin.HALF_COUNT / LFloat.Precision) +
                      LUTAsin.HALF_COUNT;
            num = Clamp(num, 0, LUTAsin.COUNT);
            return new LFloat(true,(long) LUTAsin.table[num] / 10);
        }

        //ccw
        public static LFloat Sin(LFloat radians)
        {
            int index = LUTSinCos.getIndex(radians);
            return new LFloat(true,(long) LUTSinCos.sin_table[index] / 10);
        }

        //ccw
        public static LFloat Cos(LFloat radians)
        {
            int index = LUTSinCos.getIndex(radians);
            return new LFloat(true,(long) LUTSinCos.cos_table[index] / 10);
        }
        //ccw
        public static void SinCos(out LFloat s, out LFloat c, LFloat radians)
        {
            int index = LUTSinCos.getIndex(radians);
            s = new LFloat(true,(long) LUTSinCos.sin_table[index] / 10);
            c = new LFloat(true,(long) LUTSinCos.cos_table[index] / 10);
        }

        public static uint Sqrt32(uint a)
        {
            uint num = 0u;
            uint num2 = 0u;
            for (int i = 0; i < 16; i++)
            {
                num2 <<= 1;
                num <<= 2;
                num += a >> 30;
                a <<= 2;
                if (num2 < num)
                {
                    num2 += 1u;
                    num -= num2;
                    num2 += 1u;
                }
            }

            return num2 >> 1 & 65535u;
        }

        public static ulong Sqrt64(ulong a)
        {
            ulong num = 0uL;
            ulong num2 = 0uL;
            for (int i = 0; i < 32; i++)
            {
                num2 <<= 1;
                num <<= 2;
                num += a >> 62;
                a <<= 2;
                if (num2 < num)
                {
                    num2 += 1uL;
                    num -= num2;
                    num2 += 1uL;
                }
            }

            return num2 >> 1 & 0xffffffffu;
        }

        public static int Sqrt(int a)
        {
            if (a <= 0)
            {
                return 0;
            }

            return (int) LMath.Sqrt32((uint) a);
        }

        public static int Sqrt(long a)
        {
            if (a <= 0L)
            {
                return 0;
            }

            if (a <= (long) (0xffffffffu))
            {
                return (int) LMath.Sqrt32((uint) a);
            }

            return (int) LMath.Sqrt64((ulong) a);
        }

        public static LFloat Sqrt(LFloat a)
        {
            if (a._val <= 0)
            {
                return LFloat.zero;
            }

            return new LFloat(true,Sqrt((long) a._val * LFloat.Precision));
        }

        public static LFloat Sqr(LFloat a){
            return a * a;
        }

        

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static long Clamp(long a, long min, long max)
        {
            if (a < min)
            {
                return min;
            }

            if (a > max)
            {
                return max;
            }

            return a;
        }

        public static LFloat Clamp(LFloat a, LFloat min, LFloat max)
        {
            if (a < min)
            {
                return min;
            }

            if (a > max)
            {
                return max;
            }

            return a;
        }        
        public static LFloat Clamp01(LFloat a)
        {
            if (a < LFloat.zero)
            {
                return LFloat.zero;
            }

            if (a > LFloat.one)
            {
                return LFloat.one;
            }

            return a;
        }


        public static bool SameSign(LFloat a, LFloat b)
        {
            return (long) a._val * b._val > 0L;
        }

        public static int Abs(int val)
        {
            if (val < 0)
            {
                return -val;
            }

            return val;
        }

        public static long Abs(long val)
        {
            if (val < 0L)
            {
                return -val;
            }

            return val;
        }

        public static LFloat Abs(LFloat val)
        {
            if (val._val < 0)
            {
                return new LFloat(true,-val._val);
            }

            return val;
        }

        public static int Sign(LFloat val){
            return System.Math.Sign(val._val);
        }

        public static LFloat Round(LFloat val){
            if (val <= 0) {
                var remainder = (-val._val) % LFloat.Precision;
                if (remainder > LFloat.HalfPrecision) {
                    return new LFloat(true,val._val + remainder - LFloat.Precision);
                }
                else {
                    return new LFloat(true,val._val + remainder);
                }
            }
            else {
                var remainder = (val._val) % LFloat.Precision;
                if (remainder > LFloat.HalfPrecision) {
                    return new LFloat(true,val._val - remainder + LFloat.Precision);
                }
                else {
                    return new LFloat(true,val._val - remainder);
                }
            }
        }

        public static long Max(long a, long b)
        {
            return (a <= b) ? b : a;
        }

        public static int Max(int a, int b)
        {
            return (a <= b) ? b : a;
        }

        public static long Min(long a, long b)
        {
            return (a > b) ? b : a;
        }

        public static int Min(int a, int b)
        {
            return (a > b) ? b : a;
        }
        public static int Min(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }
        public static LFloat Min(params LFloat[] values)
        {
            int length = values.Length;
            if (length == 0)
                return LFloat.zero;
            LFloat num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }
        public static int Max(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }
            return num;
        }
        
        public static LFloat Max(params LFloat[] values)
        {
            int length = values.Length;
            if (length == 0)
                return LFloat.zero;
            var num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }
            return num;
        }
        
        public static int FloorToInt(LFloat a){
            var val = a._val;
            if (val < 0) {
                val = val - LFloat.Precision + 1;
            }
            return val / LFloat.Precision ;
        }

        public static LFloat ToLFloat(float a)
        {
            return  new LFloat(true,(int)(a * LFloat.Precision));
        }
        public static LFloat ToLFloat(int a)
        {
            return  new LFloat(true,(int)(a * LFloat.Precision));
        }
        public static LFloat ToLFloat(long a)
        {
            return  new LFloat(true,(int)(a * LFloat.Precision));
        }

        public static LFloat Min(LFloat a, LFloat b)
        {
            return new LFloat(true,Min(a._val, b._val));
        }

        public static LFloat Max(LFloat a, LFloat b)
        {
            return new LFloat(true,Max(a._val, b._val));
        }

        public static LFloat Lerp(LFloat a, LFloat b, LFloat f)
        {
            return new LFloat(true,(int) (((long) (b._val - a._val) * f._val) / LFloat.Precision) + a._val);
        }

        public static LFloat InverseLerp(LFloat a, LFloat b, LFloat value)
        {
            if ( a !=  b)
                return Clamp01( (( value -  a) / ( b -  a)));
            return LFloat.zero;
        }
        public static LVector2 Lerp(LVector2 a, LVector2 b, LFloat f)
        {
            return new LVector2(true,
                (int) (((long) (b._x - a._x) * f._val) / LFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / LFloat.Precision) + a._y);
        }

        public static LVector3 Lerp(LVector3 a, LVector3 b, LFloat f)
        {
            return new LVector3(true,
                (int) (((long) (b._x - a._x) * f._val) / LFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / LFloat.Precision) + a._y,
                (int) (((long) (b._z - a._z) * f._val) / LFloat.Precision) + a._z);
        }

        public static bool IsPowerOfTwo(int x)
        {
            return (x & x - 1) == 0;
        }

        public static int CeilPowerOfTwo(int x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;
            return x;
        }

        public static LFloat Dot(LVector2 u, LVector2 v){
            return new LFloat(true,((long) u._x * v._x + (long) u._y * v._y) / LFloat.Precision);
        }

        public static LFloat Dot(LVector3 lhs, LVector3 rhs)
        {
            var val = ((long) lhs._x) * rhs._x + ((long) lhs._y) * rhs._y + ((long) lhs._z) * rhs._z;
            return new LFloat(true,val / LFloat.Precision);
            ;
        }
        public static LVector3 Cross(LVector3 lhs, LVector3 rhs)
        {
            return new LVector3(true,
                ((long) lhs._y * rhs._z - (long) lhs._z * rhs._y) / LFloat.Precision,
                ((long) lhs._z * rhs._x - (long) lhs._x * rhs._z) / LFloat.Precision,
                ((long) lhs._x * rhs._y - (long) lhs._y * rhs._x) / LFloat.Precision
            );
        }

        public static LFloat Cross2D(LVector2 u, LVector2 v)
        {
            return new LFloat(true,((long)u._x * v._y - (long)u._y * v._x) / LFloat.Precision);
        }
        public static LFloat Dot2D(LVector2 u, LVector2 v)
        {
            return new LFloat(true,((long) u._x * v._x + (long) u._y * v._y) / LFloat.Precision);
        }

    }
}