using System;
using Lockstep.Collision2D;
using Lockstep.Math;
using Ray2D = Lockstep.Collision2D.Ray2D;

namespace Lockstep.UnsafeCollision2D {
    public struct LRect {
        public LFloat x;
        public LFloat y;
        public LFloat xMax;
        public LFloat yMax;

        public LRect(LFloat x, LFloat y, LFloat width, LFloat heigh){
            this.x = x;
            this.y = y;
            this.xMax = x + width;
            this.yMax = y + heigh;
        }

        public LRect(LVector2 position, LVector2 size){
            this.x = position.x;
            this.y = position.y;
            this.xMax = x + size.x;
            this.yMax = y + size.y;
        }

        public static LRect CreateRect(LVector2 center, LVector2 halfSize){
            return new LRect(center - halfSize, halfSize * 2);
        }

        public LFloat width {
            get => xMax - x;
            set => xMax = x + width;
        }

        public LFloat height {
            get => yMax - y;
            set => yMax = y + width;
        }

        /// <summary>
        ///   <para>Shorthand for writing new LRect(0,0,0,0).</para>
        /// </summary>
        public static LRect zero {
            get { return new LRect(LFloat.zero, LFloat.zero, LFloat.zero, LFloat.zero); }
        }

        public static LRect MinMaxRect(LFloat xmin, LFloat ymin, LFloat xmax, LFloat ymax){
            return new LRect(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        public void Set(LFloat x, LFloat y, LFloat width, LFloat height){
            this.x = x;
            this.y = y;
            this.xMax = x + width;
            this.yMax = y + height;
        }

        public LVector2 position {
            get { return new LVector2(this.x, this.y); }
            set {
                this.x = value.x;
                this.y = value.y;
            }
        }

        public LVector2 center {
            get { return new LVector2((x + xMax) / 2, (y + yMax) / 2); }
            set {
                var wid = width;
                var high = height;
                this.x = value.x - width / 2;
                this.y = value.y - height / 2;
                xMax = x + wid;
                yMax = y + high;
            }
        }

        /// <summary>
        ///   <para>The position of the minimum corner of the rectangle.</para>
        /// </summary>
        public LVector2 min {
            get { return new LVector2(this.x, this.y); }
            set {
                this.x = value.x;
                this.y = value.y;
            }
        }

        /// <summary>
        ///   <para>The position of the maximum corner of the rectangle.</para>
        /// </summary>
        public LVector2 max {
            get { return new LVector2(this.xMax, this.yMax); }
            set {
                this.xMax = value.x;
                this.yMax = value.y;
            }
        }

        /// <summary>
        ///   <para>The width and height of the rectangle.</para>
        /// </summary>
        public LVector2 size {
            get { return new LVector2(xMax - x, yMax - y); }
            set {
                this.xMax = value.x + x;
                this.yMax = value.y + y;
            }
        }
        public LVector2 halfSize => new LVector2(xMax - x, yMax - y)/2;

        public bool Contains(LVector2 point){
            return point.x >= this.x && point.x < this.xMax &&
                   point.y >= this.y && point.y < this.yMax;
        }

        public bool Contains(LVector3 point){
            return point.x >= this.x && point.x < this.xMax &&
                   point.y >= this.y && point.y < this.yMax;
        }

        private static LRect OrderMinMax(LRect rect){
            if (rect.x > rect.xMax) {
                LFloat xMin = rect.x;
                rect.x = rect.xMax;
                rect.xMax = xMin;
            }

            if (rect.y > rect.yMax) {
                LFloat yMin = rect.y;
                rect.y = rect.yMax;
                rect.yMax = yMin;
            }

            return rect;
        }

        /// <summary>
        ///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the LRects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        /// <param name="other">Other rectangle to test overlapping with.</param>
        /// <param name="allowInverse">Does the test allow the widths and heights of the LRects to be negative?</param>
        public bool Overlaps(LRect other){
            return
                other.xMax > this.x
                && other.x < this.xMax
                && other.yMax > this.y
                && other.y < this.yMax;
        }
        public bool IntersectRay(Ray2D other,out LFloat distance){
            return Utils.TestRayAABB(other.origin, other.direction, min, max,out  distance);
        }
        

        /// <summary>
        ///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the LRects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
        /// </summary>
        public bool Overlaps(LRect other, bool allowInverse){
            var rect = this;
            if (allowInverse) {
                rect = LRect.OrderMinMax(rect);
                other = LRect.OrderMinMax(other);
            }

            return rect.Overlaps(other);
        }

        /// <summary>
        ///   <para>Returns a point inside a rectangle, given normalized coordinates.</para>
        /// </summary>
        public static LVector2 NormalizedToPoint(
            LRect rectangle,
            LVector2 normalizedRectCoordinates){
            return new LVector2(LMath.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x),
                LMath.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
        }


        public static bool operator !=(LRect lhs, LRect rhs){
            return !(lhs == rhs);
        }

        public static bool operator ==(LRect lhs, LRect rhs){
            return lhs.x == rhs.x && lhs.y == rhs.y &&
                   lhs.xMax == rhs.xMax && lhs.yMax == rhs.yMax;
        }

        public override int GetHashCode(){
            return this.x.GetHashCode() ^ this.xMax.GetHashCode() << 2 ^ this.y.GetHashCode() >> 2 ^
                   this.yMax.GetHashCode() >> 1;
        }

        public override bool Equals(object other){
            if (!(other is LRect))
                return false;
            return this.Equals((LRect) other);
        }

        public bool Equals(LRect other){
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.xMax.Equals(other.xMax) &&
                   this.yMax.Equals(other.yMax);
        }

        public override string ToString(){
            return
                $"(x:{(object) this.x:F2}, y:{(object) this.y:F2}, width:{(object) this.xMax:F2}, height:{(object) this.yMax:F2})";
        }
    }
}