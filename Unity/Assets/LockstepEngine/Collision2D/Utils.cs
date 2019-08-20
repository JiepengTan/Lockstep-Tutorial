using System;
using System.Collections.Generic;
using Lockstep.Math;
using static Lockstep.Math.LMath;
using Point2D = Lockstep.Math.LVector2;

namespace Lockstep.Collision2D {
    public enum ECollisionEvent {
        Enter,
        Stay,
        Exit
    }

    public enum EShape2D {
        Segment,
        Ray,
        Circle,
        AABB,
        OBB,
        Polygon,
        EnumCount,
    }

    public struct CollisionResult {
        public bool Collides;
        public LVector3 Normal;
        public LFloat Penetration;
        public ECollisionEvent Type;
        public bool First;
    }

    public  static unsafe partial class Utils {
      
        //TODO
        public static bool TestPolygonPolygon(LVector2* _points, int vertexCount, LVector2* _points2, int vertexCount2){
            return false;
        }




        //http://www.kevlindev.com/geometry/2D/intersections/index.htm
        //http://www.kevlindev.com/geometry/2D/intersections/index.htm
        //https://bitlush.com/blog/circle-vs-polygon-collision-detection-in-c-sharp
        public static bool TestCirclePolygon(LVector2 c, LFloat r, LVector2* _points, int vertexCount){
            var radiusSquared = r * r;
            var circleCenter = c;
            var nearestDistance = LFloat.MaxValue;
            int nearestVertex = -1;

            for (var i = 0; i < vertexCount; i++) {
                LVector2 axis = circleCenter - _points[i];
                var distance = axis.sqrMagnitude - radiusSquared;
                if (distance <= 0) {
                    return true;
                }

                if (distance < nearestDistance) {
                    nearestVertex = i;
                    nearestDistance = distance;
                }
            }

            LVector2 GetPoint(int index){
                if (index < 0) {
                    index += vertexCount;
                }
                else if (index >= vertexCount) {
                    index -= vertexCount;
                }

                return _points[index];
            }

            var vertex = GetPoint(nearestVertex - 1);
            for (var i = 0; i < 2; i++) {
                var nextVertex = GetPoint(nearestVertex + i);
                var edge = nextVertex - vertex;
                var edgeLengthSquared = edge.sqrMagnitude;
                if (edgeLengthSquared != 0) {
                    LVector2 axis = circleCenter - vertex;
                    var dot = LVector2.Dot(edge, axis);
                    if (dot >= 0 && dot <= edgeLengthSquared) {
                        LVector2 projection = vertex + (dot / edgeLengthSquared) * edge;
                        axis = projection - circleCenter;
                        if (axis.sqrMagnitude <= radiusSquared) {
                            return true;
                        }
                        else {
                            if (edge.x > 0) {
                                if (axis.y > 0) {
                                    return false;
                                }
                            }
                            else if (edge.x < 0) {
                                if (axis.y < 0) {
                                    return false;
                                }
                            }
                            else if (edge.y > 0) {
                                if (axis.x < 0) {
                                    return false;
                                }
                            }
                            else {
                                if (axis.x > 0) {
                                    return false;
                                }
                            }
                        }
                    }
                }

                vertex = nextVertex;
            }

            return true;
        }

        //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect  
        public static bool TestRayPolygon(LVector2 o, LVector2 dir, LVector2* points, int vertexCount){
            for (var i = 0; i < vertexCount; i++) {
                var b1 = points[i];
                var b2 = points[(i + 1) % vertexCount];
                var inter = TestRaySegment(o, dir, b1, b2);
                if (inter >= 0) {
                    return true;
                }
            }

            return false;
        }

        public static bool TestRayPolygon(LVector2 o, LVector2 dir, LVector2* points, int vertexCount,
            ref LVector2 point){
            LFloat t = LFloat.FLT_MAX;
            for (var i = 0; i < vertexCount; i++) {
                var b1 = points[i];
                var b2 = points[(i + 1) % vertexCount];
                var inter = TestRaySegment(o, dir, b1, b2);
                if (inter >= 0) {
                    if (inter < t) {
                        t = inter;
                    }
                }
            }

            if (t < LFloat.FLT_MAX) {
                point = o + dir * t;
            }

            return false;
        }


        public static LFloat TestRaySegment(LVector2 o, LVector2 d1, LVector2 p2, LVector2 p3){
            var diff = p2 - o;
            var d2 = p3 - p2;

            var demo = Cross2D(d1, d2); //det
            if (LMath.Abs(demo) < LFloat.EPSILON) //parallel
                return LFloat.negOne;

            var t1 = Cross2D(d2, diff) / demo; // Cross2D(diff,-d2)
            var t2 = Cross2D(d1, diff) / demo; //Dot(v1,pd0) == cross(d0,d1)

            if (t1 >= 0 && (t2 >= 0 && t2 <= 1))
                return t1;
            return LFloat.negOne;
        }

        public static LFloat TestSegmentSegment(LVector2 p0, LVector2 p1, LVector2 p2, LVector2 p3){
            var diff = p2 - p0;
            var d1 = p1 - p0;
            var d2 = p3 - p2;


            var demo = Cross2D(d1, d2); //det
            if (LMath.Abs(demo) < LFloat.EPSILON) //parallel
                return LFloat.negOne;

            var t1 = Cross2D(d2, diff) / demo; // Cross2D(diff,-d2)
            var t2 = Cross2D(d1, diff) / demo; //Dot(v1,pd0) == cross(d0,d1)

            if ((t1 >= 0 && t1 <= 1) && (t2 >= 0 && t2 <= 1))
                return t1; // return p0 + (p1-p0) * t1
            return LFloat.negOne;
        }
        //http://geomalgorithms.com/

//https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm
        public static bool TestRayCircle(LVector2 cPos, LFloat cR, LVector2 rB, LVector2 rDir, ref LFloat t){
            var d = rDir;
            var f = rB - cPos;
            var a = LVector2.Dot(d, d);
            var b = 2 * LVector2.Dot(f, d);
            var c = LVector2.Dot(f, f) - cR * cR;
            var discriminant = b * b - 4 * a * c;
            if (discriminant < 0) {
                // no intersection
                return false;
            }
            else {
                discriminant = LMath.Sqrt(discriminant);
                var t1 = (-b - discriminant) / (2 * a);
                var t2 = (-b + discriminant) / (2 * a);
                if (t1 >= 0) {
                    t = t1;
                    return true;
                }

                if (t2 >= 0) {
                    t = t2;
                    return true;
                }

                return false;
            }
        }

        public static bool TestRayOBB(LVector2 o, LVector2 d, LVector2 c, LVector2 size, LFloat deg,
            out LFloat tmin){
            var fo = o - c;
            fo = fo.Rotate(deg);
            var fd = d.Rotate(deg);
            return TestRayAABB(fo, fd, -size, size, out tmin);
        }

        public static bool TestRayAABB(LVector2 o, LVector2 d, LVector2 min, LVector2 max, out LFloat tmin){
            tmin = LFloat.zero; // set to -FLT_MAX to get first hit on line
            LFloat tmax = LFloat.FLT_MAX; // set to max distance ray can travel (for segment)

            // For all three slabs
            for (int i = 0; i < 2; i++) {
                if (Abs(d[i]) < LFloat.EPSILON) {
                    // Ray is parallel to slab. No hit if origin not within slab
                    if (o[i] < min[i] || o[i] > max[i]) return false;
                }
                else {
                    // Compute intersection t value of ray with near and far plane of slab
                    LFloat ood = LFloat.one / d[i];
                    LFloat t1 = (min[i] - o[i]) * ood;
                    LFloat t2 = (max[i] - o[i]) * ood;
                    // Make t1 be intersection with near plane, t2 with far plane
                    if (t1 > t2) {
                        var temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }

                    // Compute the intersection of slab intersections intervals
                    tmin = Max(tmin, t1);
                    tmax = Min(tmax, t2);
                    // Exit with no collision as soon as slab intersection becomes empty
                    if (tmin > tmax) return false;
                }
            }

            return true;
        }

        public static bool TestCircleOBB(LVector2 posA, LFloat rA, LVector2 posB, LFloat rB, LVector2 sizeB,
            LVector2 up){
            var diff = posA - posB;
            var allRadius = rA + rB;
//circle 判定CollisionHelper
            if (diff.sqrMagnitude > allRadius * allRadius) {
                return false;
            }

//空间转换
            var absX = LMath.Abs(LVector2.Dot(diff, new LVector2(up.y, -up.x)));
            var absY = LMath.Abs(LVector2.Dot(diff, up));
            var size = sizeB;
            var radius = rA;
            var x = LMath.Max(absX - size.x, LFloat.zero);
            var y = LMath.Max(absY - size.y, LFloat.zero);
            return x * x + y * y < radius * radius;
        }

        public static bool TestAABBOBB(LVector2 posA, LFloat rA, LVector2 sizeA, LVector2 posB, LFloat rB,
            LVector2 sizeB,
            LVector2 upB){
            var diff = posA - posB;
            var allRadius = rA + rB;
//circle 判定
            if (diff.sqrMagnitude > allRadius * allRadius) {
                return false;
            }

            var absUPX = LMath.Abs(upB.x); //abs(up dot aabb.right)
            var absUPY = LMath.Abs(upB.y); //abs(right dot aabb.right)
            {
//轴 投影 AABBx
                var distX = absUPX * sizeB.y + absUPY * sizeB.x;
                if (LMath.Abs(diff.x) > distX + sizeA.x) {
                    return false;
                }

//轴 投影 AABBy
//absUPX is abs(right dot aabb.up)
//absUPY is abs(up dot aabb.up)
                var distY = absUPY * sizeB.y + absUPX * sizeB.x;
                if (LMath.Abs(diff.y) > distY + sizeA.y) {
                    return false;
                }
            }
            {
                var right = new LVector2(upB.y, -upB.x);
                var diffPObbX = LVector2.Dot(diff, right);
                var diffPObbY = LVector2.Dot(diff, upB);

//absUPX is abs(aabb.up dot right )
//absUPY is abs(aabb.right dot right)
//轴 投影 OBBx
                var distX = absUPX * sizeA.y + absUPY * sizeA.x;
                if (LMath.Abs(diffPObbX) > distX + sizeB.x) {
                    return false;
                }

//absUPX is abs(aabb.right dot up )
//absUPY is abs(aabb.up dot up)
//轴 投影 OBBy
                var distY = absUPY * sizeA.y + absUPX * sizeA.x;
                if (LMath.Abs(diffPObbY) > distY + sizeB.y) {
                    return false;
                }
            }
            return true;
        }

        public static bool TestOBBOBB(LVector2 posA, LFloat rA, LVector2 sizeA, LVector2 upA, LVector2 posB,
            LFloat rB,
            LVector2 sizeB,
            LVector2 upB){
            var diff = posA - posB;
            var allRadius = rA + rB;
//circle 判定
            if (diff.sqrMagnitude > allRadius * allRadius) {
                return false;
            }

            var rightA = new LVector2(upA.y, -upA.x);
            var rightB = new LVector2(upB.y, -upB.x);
            {
//轴投影到 A.right
                var BuProjAr = LMath.Abs(LVector2.Dot(upB, rightA));
                var BrProjAr = LMath.Abs(LVector2.Dot(rightB, rightA));
                var DiffProjAr = LMath.Abs(LVector2.Dot(diff, rightA));
                var distX = BuProjAr * sizeB.y + BrProjAr * sizeB.x;
                if (DiffProjAr > distX + sizeA.x) {
                    return false;
                }

//轴投影到 A.up
                var BuProjAu = LMath.Abs(LVector2.Dot(upB, upA));
                var BrProjAu = LMath.Abs(LVector2.Dot(rightB, upA));
                var DiffProjAu = LMath.Abs(LVector2.Dot(diff, upA));
                var distY = BuProjAu * sizeB.y + BrProjAu * sizeB.x;
                if (DiffProjAu > distY + sizeA.y) {
                    return false;
                }
            }
            {
//轴投影到 B.right
                var AuProjBr = LMath.Abs(LVector2.Dot(upA, rightB));
                var ArProjBr = LMath.Abs(LVector2.Dot(rightA, rightB));
                var DiffProjBr = LMath.Abs(LVector2.Dot(diff, rightB));
                var distX = AuProjBr * sizeA.y + ArProjBr * sizeA.x;
                if (DiffProjBr > distX + sizeB.x) {
                    return false;
                }

//轴投影到 B.right
                var AuProjBu = LMath.Abs(LVector2.Dot(upA, upB));
                var ArProjBu = LMath.Abs(LVector2.Dot(rightA, upB));
                var DiffProjBu = LMath.Abs(LVector2.Dot(diff, upB));
                var distY = AuProjBu * sizeA.y + ArProjBu * sizeA.x;
                if (DiffProjBu > distY + sizeB.x) {
                    return false;
                }
            }
            return true;
        }

        public static bool TestCircleCircle(LVector2 posA, LFloat rA, LVector2 posB, LFloat rB){
            var diff = posA - posB;
            var allRadius = rA + rB;
            return diff.sqrMagnitude <= allRadius * allRadius;
        }

        public static bool TestCircleAABB(LVector2 posA, LFloat rA, LVector2 posB, LFloat rB, LVector2 sizeB){
            var diff = posA - posB;
            var allRadius = rA + rB;
//circle 判定
            if (diff.sqrMagnitude > allRadius * allRadius) {
                return false;
            }

            var absX = LMath.Abs(diff.x);
            var absY = LMath.Abs(diff.y);

//AABB & circle
            var size = sizeB;
            var radius = rA;
            var x = LMath.Max(absX - size.x, LFloat.zero);
            var y = LMath.Max(absY - size.y, LFloat.zero);
            return x * x + y * y < radius * radius;
        }

        public static bool TestAABBAABB(LVector2 posA, LFloat rA, LVector2 sizeA, LVector2 posB, LFloat rB,
            LVector2 sizeB){
            var diff = posA - posB;
            var allRadius = rA + rB;
//circle 判定
            if (diff.sqrMagnitude > allRadius * allRadius) {
                return false;
            }

            var absX = LMath.Abs(diff.x);
            var absY = LMath.Abs(diff.y);

//AABB and AABB
            var allSize = sizeA + sizeB;
            if (absX > allSize.x) return false;
            if (absY > allSize.y) return false;
            return true;
        }

        /// <summary>
        /// 判定两线段是否相交 并求交点
        /// https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#
        /// </summary>
        public static bool IntersectSegment(ref LVector2 seg1Src, ref LVector2 seg1Vec, ref LVector2 seg2Src,
            ref LVector2 seg2Vec, out LVector2 interPoint){
            interPoint = LVector2.zero;
            long denom = (long) seg1Vec._x * seg2Vec._y - (long) seg2Vec._x * seg1Vec._y; //sacle 1000
            if (denom == 0L)
                return false; // Collinear
            bool denomPositive = denom > 0L;
            var s02_x = seg1Src._x - seg2Src._x;
            var s02_y = seg1Src._y - seg2Src._y;
            long s_numer = (long) seg1Vec._x * s02_y - (long) seg1Vec._y * s02_x; //scale 1000
            if ((s_numer < 0L) == denomPositive)
                return false; // No collision
            long t_numer = seg2Vec._x * s02_y - seg2Vec._y * s02_x; //scale 1000
            if ((t_numer < 0L) == denomPositive)
                return false; // No collision
            if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
                return false; // No collision
// Collision detected
            var t = t_numer * 1000 / denom; //sacle 1000
            interPoint._x = (int) (seg1Src._x + ((long) ((t * seg1Vec._x)) / 1000));
            interPoint._y = (int) (seg1Src._y + ((long) ((t * seg1Vec._y)) / 1000));
            return true;
        }

        /// <summary>
        ///  判定点是否在多边形内
        /// https://stackoverflow.com/questions/217578/how-can-i-determine-whether-a-2d-point-is-within-a-polygon
        /// </summary>
        public static bool IsPointInPolygon(LVector2 p, LVector2[] polygon){
            var minX = polygon[0]._x;
            var maxX = polygon[0]._x;
            var minY = polygon[0]._y;
            var maxY = polygon[0]._y;
            for (int i = 1; i < polygon.Length; i++) {
                LVector2 q = polygon[i];
                minX = LMath.Min(q._x, minX);
                maxX = LMath.Max(q._x, maxX);
                minY = LMath.Min(q._y, minY);
                maxY = LMath.Max(q._y, maxY);
            }

            if (p._x < minX || p._x > maxX || p._y < minY || p._y > maxY) {
                return false;
            }

// http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++) {
                if ((polygon[i]._y > p._y) != (polygon[j]._y > p._y) &&
                    p._x < (polygon[j]._x - polygon[i]._x) * (p._y - polygon[i]._y) /
                    (polygon[j]._y - polygon[i]._y) +
                    polygon[i]._x) {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}