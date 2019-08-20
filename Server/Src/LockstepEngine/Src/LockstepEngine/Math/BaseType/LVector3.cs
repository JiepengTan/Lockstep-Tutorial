using System;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace Lockstep.Math
{
    [Serializable]
    public struct LVector3 : IEquatable<LVector3>
    {
        public LFloat x
        {
            get { return new LFloat(true,_x); }
            set { _x = value._val ; }
        }

        public LFloat y
        {
            get { return new LFloat(true,_y); }
            set { _y = value._val ; }
        }

        public LFloat z
        {
            get { return new LFloat(true,_z); }
            set { _z = value._val ; }
        }

        public int _x;
        public int _y;
        public int _z;


        public static readonly LVector3 zero = new LVector3(true,0, 0, 0);
        public static readonly LVector3 one = new LVector3(true,LFloat.Precision, LFloat.Precision, LFloat.Precision);
        public static readonly LVector3 half = new LVector3(true,LFloat.Precision / 2, LFloat.Precision / 2,LFloat.Precision / 2);
        
        public static readonly LVector3 forward = new LVector3(true,0, 0, LFloat.Precision);
        public static readonly LVector3 up = new LVector3(true,0, LFloat.Precision, 0);
        public static readonly LVector3 right = new LVector3(true,LFloat.Precision, 0, 0);
        public static readonly LVector3 back = new LVector3(true,0, 0, -LFloat.Precision);
        public static readonly LVector3 down = new LVector3(true,0, -LFloat.Precision, 0);
        public static readonly LVector3 left = new LVector3(true,-LFloat.Precision, 0, 0);
        
        /// <summary>
        /// 将这些值作为内部值 直接构造(高效) （仅用于内部实现，外部不建议使用）
        /// </summary>
        public LVector3(bool isUseRawVal,int _x, int _y, int _z)
        {
            this._x = _x;
            this._y = _y;
            this._z = _z;
        }
        /// <summary>
        /// 将这些值作为内部值 直接构造(高效) （仅用于内部实现，外部不建议使用）
        /// </summary>
        public LVector3(bool isUseRawVal,long _x, long _y, long _z)
        {
            this._x = (int) _x;
            this._y = (int) _y;
            this._z = (int) _z;
        }

        public LVector3(int _x, int _y, int _z)
        {
            this._x = _x * LFloat.Precision;
            this._y = _y * LFloat.Precision;
            this._z = _z * LFloat.Precision;
        }
        public LVector3(LFloat x, LFloat y, LFloat z)
        {
            this._x = x._val;
            this._y = y._val;
            this._z = z._val;
        }
        #if UNITY_EDITOR
        /// <summary>
        /// 直接使用浮点型 进行构造 警告!!! 仅应该在Editor模式下使用，不应该在正式代码中使用,避免出现引入浮点的不确定性
        /// </summary>
        public LVector3(bool shouldOnlyUseInEditor,float x, float y, float z)
        {
            this._x = (int)(x * LFloat.Precision);
            this._y = (int)(y * LFloat.Precision);
            this._z = (int)(z * LFloat.Precision);
        }
        #endif

        public LFloat magnitude
        {
            get
            {
                long x = (long) this._x;
                long y = (long) this._y;
                long z = (long) this._z;
                return new LFloat(true,LMath.Sqrt(x * x + y * y + z * z));
            }
        }


        public LFloat sqrMagnitude
        {
            get
            {
                long x = (long) this._x;
                long y = (long) this._y;
                long z = (long) this._z;
                return new LFloat(true,(x * x + y * y + z * z) / LFloat.Precision);
            }
        }

        public LVector3 abs
        {
            get { return new LVector3(true,LMath.Abs(this._x), LMath.Abs(this._y), LMath.Abs(this._z)); }
        }

        public LVector3 Normalize()
        {
            return Normalize((LFloat) 1);
        }

        public LVector3 Normalize(LFloat newMagn)
        {
            long num = (long) (this._x * 100);
            long num2 = (long) (this._y * 100);
            long num3 = (long) (this._z * 100);
            long num4 = num * num + num2 * num2 + num3 * num3;
            if (num4 == 0L)
            {
                return this;
            }

            long b = (long) LMath.Sqrt(num4);
            long num5 = newMagn._val;
            this._x = (int) (num * num5 / b);
            this._y = (int) (num2 * num5 / b);
            this._z = (int) (num3 * num5 / b);
            return this;
        }

        public LVector3 normalized
        {
            get
            {
                long num = (long) ((long) this._x << 7);
                long num2 = (long) ((long) this._y << 7);
                long num3 = (long) ((long) this._z << 7);
                long num4 = num * num + num2 * num2 + num3 * num3;
                if (num4 == 0L)
                {
                    return LVector3.zero;
                }

                var ret = new LVector3();
                long b = (long) LMath.Sqrt(num4);
                long num5 = LFloat.Precision;
                ret._x = (int) (num * num5 / b);
                ret._y = (int) (num2 * num5 / b);
                ret._z = (int) (num3 * num5 / b);
                return ret;
            }
        }

        public LVector3 RotateY(LFloat degree)
        {
            LFloat s;
            LFloat c;
            LMath.SinCos(out s, out c, new LFloat(true,degree._val * 31416L / 1800000L));
            LVector3 vInt;
            vInt._x = (int) (((long) this._x * s._val + (long) this._z * c._val) / LFloat.Precision);
            vInt._z = (int) (((long) this._x * -c._val + (long) this._z * s._val) / LFloat.Precision);
            vInt._y = 0;
            return vInt.normalized;
        }


        public static bool operator ==(LVector3 lhs, LVector3 rhs)
        {
            return lhs._x == rhs._x && lhs._y == rhs._y && lhs._z == rhs._z;
        }

        public static bool operator !=(LVector3 lhs, LVector3 rhs)
        {
            return lhs._x != rhs._x || lhs._y != rhs._y || lhs._z != rhs._z;
        }

        public static LVector3 operator -(LVector3 lhs, LVector3 rhs)
        {
            lhs._x -= rhs._x;
            lhs._y -= rhs._y;
            lhs._z -= rhs._z;
            return lhs;
        }

        public static LVector3 operator -(LVector3 lhs)
        {
            lhs._x = -lhs._x;
            lhs._y = -lhs._y;
            lhs._z = -lhs._z;
            return lhs;
        }

        public static LVector3 operator +(LVector3 lhs, LVector3 rhs)
        {
            lhs._x += rhs._x;
            lhs._y += rhs._y;
            lhs._z += rhs._z;
            return lhs;
        }

        public static LVector3 operator *(LVector3 lhs, LVector3 rhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._x)) / LFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._y)) / LFloat.Precision);
            lhs._z = (int) (((long) (lhs._z * rhs._z)) / LFloat.Precision);
            return lhs;
        }

        public static LVector3 operator *(LVector3 lhs, LFloat rhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._val)) / LFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._val)) / LFloat.Precision);
            lhs._z = (int) (((long) (lhs._z * rhs._val)) / LFloat.Precision);
            return lhs;
        }

        public static LVector3 operator /(LVector3 lhs, LFloat rhs)
        {
            lhs._x = (int) (((long) lhs._x * LFloat.Precision) / rhs._val);
            lhs._y = (int) (((long) lhs._y * LFloat.Precision) / rhs._val);
            lhs._z = (int) (((long) lhs._z * LFloat.Precision) / rhs._val);
            return lhs;
        }
        
        public static LVector3 operator *(LFloat rhs,LVector3 lhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._val)) / LFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._val)) / LFloat.Precision);
            lhs._z = (int) (((long) (lhs._z * rhs._val)) / LFloat.Precision);
            return lhs;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", _x * LFloat.PrecisionFactor, _y * LFloat.PrecisionFactor,
                _z * LFloat.PrecisionFactor);
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            LVector3 other = (LVector3) o;
            return this._x == other._x && this._y == other._y && this._z == other._z;
        }


        public bool Equals(LVector3 other)
        {
            return this._x == other._x && this._y == other._y && this._z == other._z;
        }


        public override int GetHashCode()
        {
            return this._x * 73856093 ^ this._y * 19349663 ^ this._z * 83492791;
        }

        
        public LFloat this[int index]

        {

            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }

            set
            {
                switch (index)
                {
                    case 0: _x = value._val; break;
                    case 1: _y = value._val;break;
                    case 2: _z = value._val;break;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }

        }

        public static LFloat Dot(ref LVector3 lhs, ref LVector3 rhs)
        {
            var val = ((long) lhs._x) * rhs._x + ((long) lhs._y) * rhs._y + ((long) lhs._z) * rhs._z;
            return new LFloat(true,val / LFloat.Precision);
        }

        public static LFloat Dot(LVector3 lhs, LVector3 rhs)
        {
            var val = ((long) lhs._x) * rhs._x + ((long) lhs._y) * rhs._y + ((long) lhs._z) * rhs._z;
            return new LFloat(true,val / LFloat.Precision);
            ;
        }
        
        public static LVector3 Cross(ref LVector3 lhs, ref LVector3 rhs)
        {
            return new LVector3(true,
                ((long) lhs._y * rhs._z - (long) lhs._z * rhs._y) / LFloat.Precision,
                ((long) lhs._z * rhs._x - (long) lhs._x * rhs._z) / LFloat.Precision,
                ((long) lhs._x * rhs._y - (long) lhs._y * rhs._x) / LFloat.Precision
            );
        }

        public static LVector3 Cross(LVector3 lhs, LVector3 rhs)
        {
            return new LVector3(true,
                ((long) lhs._y * rhs._z - (long) lhs._z * rhs._y) / LFloat.Precision,
                ((long) lhs._z * rhs._x - (long) lhs._x * rhs._z) / LFloat.Precision,
                ((long) lhs._x * rhs._y - (long) lhs._y * rhs._x) / LFloat.Precision
            );
        }
        
        
        public static LVector3 Lerp(LVector3 a, LVector3 b, LFloat f)
        {
            return new LVector3(true,
                (int) (((long) (b._x - a._x) * f._val) / LFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / LFloat.Precision) + a._y,
                (int) (((long) (b._z - a._z) * f._val) / LFloat.Precision) + a._z);
        }
    }
}