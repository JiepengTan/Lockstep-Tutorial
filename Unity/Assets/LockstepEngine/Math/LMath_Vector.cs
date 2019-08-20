using System;
using Lockstep.Math;
using static Lockstep.Math.LVector3;

namespace Lockstep.Math
{
    public static partial class LMath
    {
        public static LVector3 Transform(ref LVector3 point, ref LVector3 axis_x, ref LVector3 axis_y, ref LVector3 axis_z,
            ref LVector3 trans)
        {
            return new LVector3(true,
                ((axis_x._x * point._x + axis_y._x * point._y + axis_z._x * point._z) / LFloat.Precision) + trans._x,
                ((axis_x._y * point._x + axis_y._y * point._y + axis_z._y * point._z) / LFloat.Precision) + trans._y,
                ((axis_x._z * point._x + axis_y._z * point._y + axis_z._z * point._z) / LFloat.Precision) + trans._z);
        }

        public static LVector3 Transform(LVector3 point, ref LVector3 axis_x, ref LVector3 axis_y, ref LVector3 axis_z,
            ref LVector3 trans)
        {
            return new LVector3(true,
                ((axis_x._x * point._x + axis_y._x * point._y + axis_z._x * point._z) / LFloat.Precision) + trans._x,
                ((axis_x._y * point._x + axis_y._y * point._y + axis_z._y * point._z) / LFloat.Precision) + trans._y,
                ((axis_x._z * point._x + axis_y._z * point._y + axis_z._z * point._z) / LFloat.Precision) + trans._z);
        }

        public static LVector3 Transform(ref LVector3 point, ref LVector3 axis_x, ref LVector3 axis_y, ref LVector3 axis_z,
            ref LVector3 trans, ref LVector3 scale)
        {
            long num = (long) point._x * (long) scale._x;
            long num2 = (long) point._y * (long) scale._x;
            long num3 = (long) point._z * (long) scale._x;
            return new LVector3(true,
                (int) (((long) axis_x._x * num + (long) axis_y._x * num2 + (long) axis_z._x * num3) / 1000000L) +
                trans._x,
                (int) (((long) axis_x._y * num + (long) axis_y._y * num2 + (long) axis_z._y * num3) / 1000000L) +
                trans._y,
                (int) (((long) axis_x._z * num + (long) axis_y._z * num2 + (long) axis_z._z * num3) / 1000000L) +
                trans._z);
        }

        public static LVector3 Transform(ref LVector3 point, ref LVector3 forward, ref LVector3 trans)
        {
            LVector3 up = LVector3.up;
            LVector3 vInt = Cross(LVector3.up, forward);
            return LMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
        }

        public static LVector3 Transform(LVector3 point, LVector3 forward, LVector3 trans)
        {
            LVector3 up = LVector3.up;
            LVector3 vInt = Cross(LVector3.up, forward);
            return LMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
        }

        public static LVector3 Transform(LVector3 point, LVector3 forward, LVector3 trans, LVector3 scale)
        {
            LVector3 up = LVector3.up;
            LVector3 vInt = Cross(LVector3.up, forward);
            return LMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans, ref scale);
        }

        public static LVector3 MoveTowards(LVector3 from, LVector3 to, LFloat dt)
        {
            if ((to - from).sqrMagnitude <= (dt * dt))
            {
                return to;
            }

            return from + (to - from).Normalize(dt);
        }


        public static LFloat AngleInt(LVector3 lhs, LVector3 rhs)
        {
            return LMath.Acos(Dot(lhs, rhs));
        }

        
        
    }
}