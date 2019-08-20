using Lockstep.Math;

namespace Lockstep.PathFinding {
    public struct SplitPlane {
        public SplitPlane(LVector2 a, LVector2 b){
            this.a = a;
            this.b = b;
        }

        public LVector2 a;
        public LVector2 b;
        public LVector2 dir => b - a;

        private static LFloat val;
        public static ESplitType GetSplitResult(SplitPlane plane, TriRef tri){
            var planeDir = plane.dir;
            var valA = LVector2.Cross(planeDir, tri.a - plane.a);
            var valB = LVector2.Cross(planeDir, tri.b - plane.a);
            var valC = LVector2.Cross(planeDir, tri.c - plane.a);

            var isRight = false;
            if (valA != 0) isRight = valA < 0;
            if (valB != 0) isRight = valB < 0;
            if (valC != 0) isRight = valC < 0;
            
            var isA = valA <= 0;
            var isB = valB <= 0;
            var isC = valC <= 0;
            if (isA == isB && isB == isC) {
                return isRight ? ESplitType.Right : ESplitType.Left;
            }

            isA = valA >= 0;
            isB = valB >= 0;
            isC = valC >= 0;
            if (isA == isB && isB == isC) {
                return isRight ? ESplitType.Right : ESplitType.Left;
            }

            return ESplitType.OnPlane;
        }

        public static ESplitType ClassifyPointToPlane(SplitPlane plane, LVector2 vertex){
            var val = LVector2.Cross(plane.dir, vertex - plane.a);
            if (val == 0)
                return ESplitType.OnPlane;
            else {
                return val < 0 ? ESplitType.Right : ESplitType.Left;
            }
        }

        public static LVector2 GetIntersectPoint(LVector2 p0, LVector2 p1, LVector2 p2, LVector2 p3){
            var diff = p2 - p0;
            var d1 = p1 - p0;
            var d2 = p3 - p2;
            var demo = LMath.Cross2D(d1, d2); //det
            if (LMath.Abs(demo) < LFloat.EPSILON) //parallel
                return p0;

            var t1 = LMath.Cross2D(diff, d2) / demo; // Cross2D(diff,-d2)
            return p0 + (p1 - p0) * t1;
        }
    }
}