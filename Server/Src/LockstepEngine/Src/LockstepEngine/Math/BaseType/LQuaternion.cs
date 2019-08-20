using System;
using Lockstep.Math;
using static Lockstep.Math.LVector3;

namespace LockStepLMath {
    public struct LQuaternion {
        #region public members

        public LFloat x;
        public LFloat y;
        public LFloat z;
        public LFloat w;

        #endregion

        #region constructor

        public LQuaternion(LFloat p_x, LFloat p_y, LFloat p_z, LFloat p_w){
            x = p_x;
            y = p_y;
            z = p_z;
            w = p_w;
        }

        public LQuaternion(int p_x, int p_y, int p_z, int p_w){
            x._val = p_x;
            y._val = p_y;
            z._val = p_z;
            w._val = p_w;
        }

        #endregion

        #region public properties

        public LFloat this[int index] {
            get {
                switch (index) {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid LQuaternion index!");
                }
            }
            set {
                switch (index) {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid LQuaternion index!");
                }
            }
        }

        public static LQuaternion identity {
            get { return new LQuaternion(0, 0, 0, 1); }
        }

        public LVector3 eulerAngles {
            get {
                LMatrix33 m = QuaternionToMatrix(this);
                return (180 / LMath.PI * MatrixToEuler(m));
            }
            set { this = Euler(value); }
        }

        #endregion

        #region public functions

        /// <summary>
        /// 夹角大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static LFloat Angle(LQuaternion a, LQuaternion b){
            LFloat single = Dot(a, b);
            return LMath.Acos(LMath.Min(LMath.Abs(single), LFloat.one)) * 2 * (180 / LMath.PI);
        }

        /// <summary>
        /// 轴向旋转
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static LQuaternion AngleAxis(LFloat angle, LVector3 axis){
            axis = axis.normalized;
            angle = angle * LMath.Deg2Rad;

            LQuaternion q = new LQuaternion();

            LFloat halfAngle = angle * LFloat.half;
            LFloat s = LMath.Sin(halfAngle);

            q.w = LMath.Cos(halfAngle);
            q.x = s * axis.x;
            q.y = s * axis.y;
            q.z = s * axis.z;

            return q;
        }

        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static LFloat Dot(LQuaternion a, LQuaternion b){
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 欧拉角转四元数
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public static LQuaternion Euler(LVector3 euler){
            return Euler(euler.x, euler.y, euler.z);
        }

        /// <summary>
        /// 欧拉角转四元数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static LQuaternion Euler(LFloat x, LFloat y, LFloat z){
            LFloat cX = LMath.Cos(x * LMath.PI / 360);
            LFloat sX = LMath.Sin(x * LMath.PI / 360);

            LFloat cY = LMath.Cos(y * LMath.PI / 360);
            LFloat sY = LMath.Sin(y * LMath.PI / 360);

            LFloat cZ = LMath.Cos(z * LMath.PI / 360);
            LFloat sZ = LMath.Sin(z * LMath.PI / 360);

            LQuaternion qX = new LQuaternion(sX, LFloat.zero, LFloat.zero, cX);
            LQuaternion qY = new LQuaternion(LFloat.zero, sY, LFloat.zero, cY);
            LQuaternion qZ = new LQuaternion(LFloat.zero, LFloat.zero, sZ, cZ);

            LQuaternion q = (qY * qX) * qZ;

            return q;
        }

        /// <summary>
        /// 向量间的角度
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        /// <returns></returns>
        public static LQuaternion FromToRotation(LVector3 fromDirection, LVector3 toDirection){
            throw new IndexOutOfRangeException("Not Available!");
        }

        /// <summary>
        /// 四元数的逆
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static LQuaternion Inverse(LQuaternion rotation){
            return new LQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static LQuaternion Lerp(LQuaternion a, LQuaternion b, LFloat t){
            if (t > 1) {
                t = LFloat.one;
            }

            if (t < 0) {
                t = LFloat.zero;
            }

            return LerpUnclamped(a, b, t);
        }

        /// <summary>
        /// 线性插值(无限制)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static LQuaternion LerpUnclamped(LQuaternion a, LQuaternion b, LFloat t){
            LQuaternion tmpQuat = new LQuaternion();
            if (Dot(a, b) < 0) {
                tmpQuat.Set(a.x + t * (-b.x - a.x),
                    a.y + t * (-b.y - a.y),
                    a.z + t * (-b.z - a.z),
                    a.w + t * (-b.w - a.w));
            }
            else {
                tmpQuat.Set(a.x + t * (b.x - a.x),
                    a.y + t * (b.y - a.y),
                    a.z + t * (b.z - a.z),
                    a.w + t * (b.w - a.w));
            }

            LFloat nor = LMath.Sqrt(Dot(tmpQuat, tmpQuat));
            return new LQuaternion(tmpQuat.x / nor, tmpQuat.y / nor, tmpQuat.z / nor, tmpQuat.w / nor);
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static LQuaternion LookRotation(LVector3 forward){
            LVector3 up = LVector3.up;
            return LookRotation(forward, up);
        }

        /// <summary>
        /// 注视旋转
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="upwards"></param>
        /// <returns></returns>
        public static LQuaternion LookRotation(LVector3 forward, LVector3 upwards){
            LMatrix33 m = LookRotationToMatrix(forward, upwards);
            return MatrixToQuaternion(m);
        }

        /// <summary>
        /// 向目标角度旋转
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        /// <returns></returns>
        public static LQuaternion RotateTowards(LQuaternion from, LQuaternion to, LFloat maxDegreesDelta){
            LFloat num = LQuaternion.Angle(from, to);
            LQuaternion result = new LQuaternion();
            if (num == 0) {
                result = to;
            }
            else {
                LFloat t = LMath.Min(LFloat.one, maxDegreesDelta / num);
                result = LQuaternion.SlerpUnclamped(from, to, t);
            }

            return result;
        }

        /// <summary>
        /// 球形插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static LQuaternion Slerp(LQuaternion a, LQuaternion b, LFloat t){
            if (t > 1) {
                t = LFloat.one;
            }

            if (t < 0) {
                t = LFloat.zero;
            }

            return SlerpUnclamped(a, b, t);
        }

        /// <summary>
        /// 球形插值(无限制)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static LQuaternion SlerpUnclamped(LQuaternion q1, LQuaternion q2, LFloat t){
            LFloat dot = Dot(q1, q2);

            LQuaternion tmpQuat = new LQuaternion();
            if (dot < 0) {
                dot = -dot;
                tmpQuat.Set(-q2.x, -q2.y, -q2.z, -q2.w);
            }
            else
                tmpQuat = q2;


            if (dot < 1) {
                LFloat angle = LMath.Acos(dot);
                LFloat sinadiv, sinat, sinaomt;
                sinadiv = 1 / LMath.Sin(angle);
                sinat = LMath.Sin(angle * t);
                sinaomt = LMath.Sin(angle * (1 - t));
                tmpQuat.Set((q1.x * sinaomt + tmpQuat.x * sinat) * sinadiv,
                    (q1.y * sinaomt + tmpQuat.y * sinat) * sinadiv,
                    (q1.z * sinaomt + tmpQuat.z * sinat) * sinadiv,
                    (q1.w * sinaomt + tmpQuat.w * sinat) * sinadiv);
                return tmpQuat;
            }
            else {
                return Lerp(q1, tmpQuat, t);
            }
        }

        /// <summary>
        /// 设置四元数
        /// </summary>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        /// <param name="new_w"></param>
        public void Set(LFloat new_x, LFloat new_y, LFloat new_z, LFloat new_w){
            x = new_x;
            y = new_y;
            z = new_z;
            w = new_w;
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        public void SetFromToRotation(LVector3 fromDirection, LVector3 toDirection){
            this = FromToRotation(fromDirection, toDirection);
        }

        /// <summary>
        /// 设置注视旋转
        /// </summary>
        /// <param name="view"></param>
        public void SetLookRotation(LVector3 view){
            this = LookRotation(view);
        }

        /// <summary>
        /// 设置注视旋转
        /// </summary>
        /// <param name="view"></param>
        /// <param name="up"></param>
        public void SetLookRotation(LVector3 view,  LVector3 up){
            this = LookRotation(view, up);
        }

        /// <summary>
        /// 转换为角轴
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="axis"></param>
        public void ToAngleAxis(out LFloat angle, out LVector3 axis){
            angle = 2 * LMath.Acos(w);
            if (angle == 0) {
                axis = LVector3.right;
                return;
            }

            LFloat div = 1 / LMath.Sqrt(1 - w * w);
            axis = new LVector3(x * div, y * div, z * div);
            angle = angle * 180 / LMath.PI;
        }

        public override string ToString(){
            return String.Format("({0}, {1}, {2}, {3})", x, y, z, w);
        }

        public override int GetHashCode(){
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^
                   this.w.GetHashCode() >> 1;
        }

        public override bool Equals(object other){
            return this == (LQuaternion) other;
        }

        #endregion

        #region private functions

        private LVector3 MatrixToEuler(LMatrix33 m){
            LVector3 v = new LVector3();
            if (m[1, 2] < 1) {
                if (m[1, 2] > -1) {
                    v.x = LMath.Asin(-m[1, 2]);
                    v.y = LMath.Atan2(m[0, 2], m[2, 2]);
                    v.z = LMath.Atan2(m[1, 0], m[1, 1]);
                }
                else {
                    v.x = LMath.PI * LFloat.half;
                    v.y = LMath.Atan2(m[0, 1], m[0, 0]);
                    v.z = (LFloat) 0;
                }
            }
            else {
                v.x = -LMath.PI * LFloat.half;
                v.y = LMath.Atan2(-m[0, 1], m[0, 0]);
                v.z = (LFloat) 0;
            }

            for (int i = 0; i < 3; i++) {
                if (v[i] < 0) {
                    v[i] += LMath.PI2;
                }
                else if (v[i] > LMath.PI2) {
                    v[i] -= LMath.PI2;
                }
            }

            return v;
        }

        public static LMatrix33 QuaternionToMatrix(LQuaternion quat){
            LMatrix33 m = new LMatrix33();

            LFloat x = quat.x * 2;
            LFloat y = quat.y * 2;
            LFloat z = quat.z * 2;
            LFloat xx = quat.x * x;
            LFloat yy = quat.y * y;
            LFloat zz = quat.z * z;
            LFloat xy = quat.x * y;
            LFloat xz = quat.x * z;
            LFloat yz = quat.y * z;
            LFloat wx = quat.w * x;
            LFloat wy = quat.w * y;
            LFloat wz = quat.w * z;

            m[0] = 1 - (yy + zz);
            m[1] = xy + wz;
            m[2] = xz - wy;

            m[3] = xy - wz;
            m[4] = 1 - (xx + zz);
            m[5] = yz + wx;

            m[6] = xz + wy;
            m[7] = yz - wx;
            m[8] = 1 - (xx + yy);

            return m;
        }

        private static LQuaternion MatrixToQuaternion(LMatrix33 m){
            LQuaternion quat = new LQuaternion();

            LFloat fTrace = m[0, 0] + m[1, 1] + m[2, 2];
            LFloat root;

            if (fTrace > 0) {
                root = LMath.Sqrt(fTrace + 1);
                quat.w = LFloat.half * root;
                root = LFloat.half / root;
                quat.x = (m[2, 1] - m[1, 2]) * root;
                quat.y = (m[0, 2] - m[2, 0]) * root;
                quat.z = (m[1, 0] - m[0, 1]) * root;
            }
            else {
                int[] s_iNext = new int[] {1, 2, 0};
                int i = 0;
                if (m[1, 1] > m[0, 0]) {
                    i = 1;
                }

                if (m[2, 2] > m[i, i]) {
                    i = 2;
                }

                int j = s_iNext[i];
                int k = s_iNext[j];

                root = LMath.Sqrt(m[i, i] - m[j, j] - m[k, k] + 1);
                if (root < 0) {
                    throw new IndexOutOfRangeException("error!");
                }

                quat[i] = LFloat.half * root;
                root = LFloat.half / root;
                quat.w = (m[k, j] - m[j, k]) * root;
                quat[j] = (m[j, i] + m[i, j]) * root;
                quat[k] = (m[k, i] + m[i, k]) * root;
            }

            LFloat nor = LMath.Sqrt(Dot(quat, quat));
            quat = new LQuaternion(quat.x / nor, quat.y / nor, quat.z / nor, quat.w / nor);

            return quat;
        }

        private static LMatrix33 LookRotationToMatrix(LVector3 viewVec, LVector3 upVec){
            LVector3 z = viewVec;
            LMatrix33 m = new LMatrix33();

            LFloat mag = z.magnitude;
            if (mag <= 0) {
                m = LMatrix33.identity;
            }

            z /= mag;

            LVector3 x = Cross(upVec, z);
            mag = x.magnitude;
            if (mag <= 0) {
                m = LMatrix33.identity;
            }

            x /= mag;

            LVector3 y = Cross(z, x);

            m[0, 0] = x.x;
            m[0, 1] = y.x;
            m[0, 2] = z.x;
            m[1, 0] = x.y;
            m[1, 1] = y.y;
            m[1, 2] = z.y;
            m[2, 0] = x.z;
            m[2, 1] = y.z;
            m[2, 2] = z.z;

            return m;
        }

        #endregion

        #region operator

        public static LQuaternion operator *(LQuaternion lhs, LQuaternion rhs){
            return new LQuaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        public static LVector3 operator *(LQuaternion rotation, LVector3 point){
            LFloat _2x = rotation.x * 2;
            LFloat _2y = rotation.y * 2;
            LFloat _2z = rotation.z * 2;
            LFloat _2xx = rotation.x * _2x;
            LFloat _2yy = rotation.y * _2y;
            LFloat _2zz = rotation.z * _2z;
            LFloat _2xy = rotation.x * _2y;
            LFloat _2xz = rotation.x * _2z;
            LFloat _2yz = rotation.y * _2z;
            LFloat _2xw = rotation.w * _2x;
            LFloat _2yw = rotation.w * _2y;
            LFloat _2zw = rotation.w * _2z;
            var x = (1 - (_2yy + _2zz)) * point.x + (_2xy - _2zw) * point.y + (_2xz + _2yw) * point.z;
            var y = (_2xy + _2zw) * point.x + (1 - (_2xx + _2zz)) * point.y + (_2yz - _2xw) * point.z;
            var z = (_2xz - _2yw) * point.x + (_2yz + _2xw) * point.y + (1 - (_2xx + _2yy)) * point.z;
            return new LVector3(x, y, z);
        }

        public static bool operator ==(LQuaternion lhs, LQuaternion rhs){
            var isEqu = lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;

            return isEqu;
        }

        public static bool operator !=(LQuaternion lhs, LQuaternion rhs){
            return !(lhs == rhs);
        }

        #endregion
    }
}
/*
*/