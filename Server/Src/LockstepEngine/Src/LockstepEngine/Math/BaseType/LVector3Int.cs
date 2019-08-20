// Decompiled with JetBrains decompiler
// Type: UnityEngine.Vector3Int
// Assembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6A5F7498-719A-42F7-B77A-5934DC79A5E9
// Assembly location: /Users/jiepengtan/Projects/LockstepDemo/Common/Libs/UnityEngine.dll

using System;
using Lockstep.Math;

namespace Lockstep.Math {
    /// <summary>
    ///   <para>Representation of 3D vectors and points using integers.</para>
    /// </summary>
    public struct LVector3Int : IEquatable<LVector3Int> {
        public class Mathf {
            /// <summary>
            ///   <para>Returns the smallest of two or more values.</para>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="values"></param>
            public static int Min(int a, int b){
                return a >= b ? b : a;
            }

            /// <summary>
            ///   <para>Returns the largest of two or more values.</para>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="values"></param>
            public static int Max(int a, int b){
                return a <= b ? b : a;
            }  
            public static LFloat Sqrt(LFloat val){
                return Lockstep.Math.LMath.Sqrt(val);
            }
        }

        private static readonly LVector3Int s_Zero = new LVector3Int(0, 0, 0);
        private static readonly LVector3Int s_One = new LVector3Int(1, 1, 1);
        private static readonly LVector3Int s_Up = new LVector3Int(0, 1, 0);
        private static readonly LVector3Int s_Down = new LVector3Int(0, -1, 0);
        private static readonly LVector3Int s_Left = new LVector3Int(-1, 0, 0);
        private static readonly LVector3Int s_Right = new LVector3Int(1, 0, 0);
        private int m_X;
        private int m_Y;
        private int m_Z;

        public LVector3Int(int x, int y, int z){
            this.m_X = x;
            this.m_Y = y;
            this.m_Z = z;
        }

        /// <summary>
        ///   <para>X component of the vector.</para>
        /// </summary>
        public int x {
            get { return this.m_X; }
            set { this.m_X = value; }
        }

        /// <summary>
        ///   <para>Y component of the vector.</para>
        /// </summary>
        public int y {
            get { return this.m_Y; }
            set { this.m_Y = value; }
        }

        /// <summary>
        ///   <para>Z component of the vector.</para>
        /// </summary>
        public int z {
            get { return this.m_Z; }
            set { this.m_Z = value; }
        }

        /// <summary>
        ///   <para>Set x, y and z components of an existing Vector3Int.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(int x, int y, int z){
            this.m_X = x;
            this.m_Y = y;
            this.m_Z = z;
        }

        public int this[int index] {
            get {
                switch (index) {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!",
                            (object) index));
                }
            }
            set {
                switch (index) {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(
                            string.Format("Invalid Vector3Int index addressed: {0}!", (object) index));
                }
            }
        }

        /// <summary>
        ///   <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public LFloat magnitude {
            get { return Mathf.Sqrt(new LFloat(this.x * this.x + this.y * this.y + this.z * this.z)); }
        }
     
        /// <summary>
        ///   <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public int sqrMagnitude {
            get { return this.x * this.x + this.y * this.y + this.z * this.z; }
        }

        /// <summary>
        ///   <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static LFloat Distance(LVector3Int a, LVector3Int b){
            return (a - b).magnitude;
        }

        /// <summary>
        ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static LVector3Int Min(LVector3Int lhs, LVector3Int rhs){
            return new LVector3Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
        }

        /// <summary>
        ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static LVector3Int Max(LVector3Int lhs, LVector3Int rhs){
            return new LVector3Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
        }

        /// <summary>
        ///   <para>Multiplies two vectors component-wise.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static LVector3Int Scale(LVector3Int a, LVector3Int b){
            return new LVector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        ///   <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(LVector3Int scale){
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        /// <summary>
        ///   <para>Clamps the Vector3Int to the bounds given by min and max.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Clamp(LVector3Int min, LVector3Int max){
            this.x = LMath.Max(min.x, this.x);
            this.x = LMath.Min(max.x, this.x);
            this.y = LMath.Max(min.y, this.y);
            this.y = LMath.Min(max.y, this.y);
            this.z = LMath.Max(min.z, this.z);
            this.z = LMath.Min(max.z, this.z);
        }

        public static explicit operator LVector2Int(LVector3Int v){
            return new LVector2Int(v.x, v.y);
        }


        public static LVector3Int operator +(LVector3Int a, LVector3Int b){
            return new LVector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static LVector3Int operator -(LVector3Int a, LVector3Int b){
            return new LVector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static LVector3Int operator *(LVector3Int a, LVector3Int b){
            return new LVector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static LVector3Int operator *(LVector3Int a, int b){
            return new LVector3Int(a.x * b, a.y * b, a.z * b);
        }

        public static bool operator ==(LVector3Int lhs, LVector3Int rhs){
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(LVector3Int lhs, LVector3Int rhs){
            return !(lhs == rhs);
        }

        /// <summary>
        ///   <para>Returns true if the objects are equal.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object other){
            if (!(other is LVector3Int))
                return false;
            return this.Equals((LVector3Int) other);
        }

        public bool Equals(LVector3Int other){
            return this == other;
        }

        /// <summary>
        ///   <para>Gets the hash code for the Vector3Int.</para>
        /// </summary>
        /// <returns>
        ///   <para>The hash code of the Vector3Int.</para>
        /// </returns>
        public override int GetHashCode(){
            int hashCode1 = this.y.GetHashCode();
            int hashCode2 = this.z.GetHashCode();
            return this.x.GetHashCode() ^ hashCode1 << 4 ^ hashCode1 >> 28 ^ hashCode2 >> 4 ^ hashCode2 << 28;
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString(){
            return string.Format("({0}, {1}, {2})", (object) this.x, (object) this.y, (object) this.z);
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for this vector.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format){
            return string.Format("({0}, {1}, {2})", (object) this.x.ToString(format),
                (object) this.y.ToString(format), (object) this.z.ToString(format));
        }

        /// <summary>
        ///   <para>Shorthand for writing Vector3Int (0, 0, 0).</para>
        /// </summary>
        public static LVector3Int zero {
            get { return LVector3Int.s_Zero; }
        }

        /// <summary>
        ///   <para>Shorthand for writing Vector3Int (1, 1, 1).</para>
        /// </summary>
        public static LVector3Int one {
            get { return LVector3Int.s_One; }
        }

        /// <summary>
        ///   <para>Shorthand for writing Vector3Int (0, 1, 0).</para>
        /// </summary>
        public static LVector3Int up {
            get { return LVector3Int.s_Up; }
        }

        /// <summary>
        ///   <para>Shorthand for writing Vector3Int (0, -1, 0).</para>
        /// </summary>
        public static LVector3Int down {
            get { return LVector3Int.s_Down; }
        }

        /// <summary>
        ///   <para>Shorthand for writing Vector3Int (-1, 0, 0).</para>
        /// </summary>
        public static LVector3Int left {
            get { return LVector3Int.s_Left; }
        }

        /// <summary>
        ///   <para>Shorthand for writing Vector3Int (1, 0, 0).</para>
        /// </summary>
        public static LVector3Int right {
            get { return LVector3Int.s_Right; }
        }
    }
}