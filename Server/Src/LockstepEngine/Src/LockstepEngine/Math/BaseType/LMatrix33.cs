using Lockstep.Math;
using static Lockstep.Math.LMath;
using System;

namespace Lockstep.Math
{
    public struct LMatrix33 : IEquatable<LMatrix33>
    {
        public static readonly LMatrix33
            zero = new LMatrix33(LVector3.zero, LVector3.zero, LVector3.zero);

        public static readonly LMatrix33 identity = new LMatrix33(new LVector3(true,LFloat.Precision, 0, 0),
            new LVector3(true,0, LFloat.Precision, 0), new LVector3(true,0, 0, LFloat.Precision));

        // mRowCol  列优先存储
        public int m00;
        public int m10;
        public int m20;
        public int m01;
        public int m11;
        public int m21;
        public int m02;
        public int m12;
        public int m22;

        public LMatrix33(LVector3 column0, LVector3 column1, LVector3 column2)
        {
            this.m00 = column0._x;
            this.m01 = column1._x;
            this.m02 = column2._x;
            this.m10 = column0._y;
            this.m11 = column1._y;
            this.m12 = column2._y;
            this.m20 = column0._z;
            this.m21 = column1._z;
            this.m22 = column2._z;
        }


        public LFloat this[int row, int column]
        {
            get { return this[row + column * 3]; }
            set { this[row + column * 3] = value; }
        }

        public LFloat this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return new LFloat(true,this.m00);
                    case 1:
                        return new LFloat(true,this.m10);
                    case 2:
                        return new LFloat(true,this.m20);
                    case 3:
                        return new LFloat(true,this.m01);
                    case 4:
                        return new LFloat(true,this.m11);
                    case 5:
                        return new LFloat(true,this.m21);
                    case 6:
                        return new LFloat(true,this.m02);
                    case 7:
                        return new LFloat(true,this.m12);
                    case 8:
                        return new LFloat(true,this.m22);
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.m00 = value._val;
                        break;
                    case 1:
                        this.m10 = value._val;
                        break;
                    case 2:
                        this.m20 = value._val;
                        break;
                    case 3:
                        this.m01 = value._val;
                        break;
                    case 4:
                        this.m11 = value._val;
                        break;
                    case 5:
                        this.m21 = value._val;
                        break;
                    case 6:
                        this.m02 = value._val;
                        break;
                    case 7:
                        this.m12 = value._val;
                        break;
                    case 8:
                        this.m22 = value._val;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public override int GetHashCode()
        {
            return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^
                   this.GetColumn(2).GetHashCode() >> 2;
        }

        public override bool Equals(object other)
        {
            if (!(other is LMatrix33))
                return false;
            return this.Equals((LMatrix33) other);
        }

        public bool Equals(LMatrix33 other)
        {
            return this.GetColumn(0).Equals(other.GetColumn(0))
                   && this.GetColumn(1).Equals(other.GetColumn(1))
                   && this.GetColumn(2).Equals(other.GetColumn(2));
        }

        public static LMatrix33 operator *(LMatrix33 lhs, LMatrix33 rhs)
        {
            LMatrix33 mat;
            mat.m00 = (int) (((long) lhs.m00 * (long) rhs.m00 + (long) lhs.m01 * (long) rhs.m10 +
                              (long) lhs.m02 * (long) rhs.m20) / LFloat.Precision);
            mat.m01 = (int) (((long) lhs.m00 * (long) rhs.m01 + (long) lhs.m01 * (long) rhs.m11 +
                              (long) lhs.m02 * (long) rhs.m21) / LFloat.Precision);
            mat.m02 = (int) (((long) lhs.m00 * (long) rhs.m02 + (long) lhs.m01 * (long) rhs.m12 +
                              (long) lhs.m02 * (long) rhs.m22) / LFloat.Precision);
            mat.m10 = (int) (((long) lhs.m10 * (long) rhs.m00 + (long) lhs.m11 * (long) rhs.m10 +
                              (long) lhs.m12 * (long) rhs.m20) / LFloat.Precision);
            mat.m11 = (int) (((long) lhs.m10 * (long) rhs.m01 + (long) lhs.m11 * (long) rhs.m11 +
                              (long) lhs.m12 * (long) rhs.m21) / LFloat.Precision);
            mat.m12 = (int) (((long) lhs.m10 * (long) rhs.m02 + (long) lhs.m11 * (long) rhs.m12 +
                              (long) lhs.m12 * (long) rhs.m22) / LFloat.Precision);
            mat.m20 = (int) (((long) lhs.m20 * (long) rhs.m00 + (long) lhs.m21 * (long) rhs.m10 +
                              (long) lhs.m22 * (long) rhs.m20) / LFloat.Precision);
            mat.m21 = (int) (((long) lhs.m20 * (long) rhs.m01 + (long) lhs.m21 * (long) rhs.m11 +
                              (long) lhs.m22 * (long) rhs.m21) / LFloat.Precision);
            mat.m22 = (int) (((long) lhs.m20 * (long) rhs.m02 + (long) lhs.m21 * (long) rhs.m12 +
                              (long) lhs.m22 * (long) rhs.m22) / LFloat.Precision);
            return mat;
        }

        public static LVector3 operator *(LMatrix33 lhs, LVector3 vector3)
        {
            LVector3 vec;
            vec._x = (int) (((long) lhs.m00 * (long) vector3.x + (long) lhs.m01 * (long) vector3.y +
                             (long) lhs.m02 * (long) vector3.z) / LFloat.Precision);
            vec._y = (int) (((long) lhs.m10 * (long) vector3.x + (long) lhs.m11 * (long) vector3.y +
                             (long) lhs.m12 * (long) vector3.z) / LFloat.Precision);
            vec._z = (int) (((long) lhs.m20 * (long) vector3.x + (long) lhs.m21 * (long) vector3.y +
                             (long) lhs.m22 * (long) vector3.z) / LFloat.Precision);
            return vec;
        }

        public static bool operator ==(LMatrix33 lhs, LMatrix33 rhs)
        {
            return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) &&
                   lhs.GetColumn(2) == rhs.GetColumn(2);
        }

        public static bool operator !=(LMatrix33 lhs, LMatrix33 rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        ///   <para>Get a column of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        public LVector3 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return new LVector3(true,this.m00, this.m10, this.m20);
                case 1:
                    return new LVector3(true,this.m01, this.m11, this.m21);
                case 2:
                    return new LVector3(true,this.m02, this.m12, this.m22);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        /// <summary>
        ///   <para>Returns a row of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        public LVector3 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return new LVector3(true,this.m00, this.m01, this.m02);
                case 1:
                    return new LVector3(true,this.m10, this.m11, this.m12);
                case 2:
                    return new LVector3(true,this.m20, this.m21, this.m22);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        /// <summary>
        ///   <para>Sets a column of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="column"></param>
        public void SetColumn(int index, LVector3 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
        }

        /// <summary>
        ///   <para>Sets a row of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="row"></param>
        public void SetRow(int index, LVector3 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
        }
    }
}