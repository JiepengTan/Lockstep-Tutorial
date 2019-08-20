using System;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public class Plane {
        private static long serialVersionUID = -1240652082930747866L;

        public LVector3 normal = new LVector3(); // 单位长度
        public LFloat d = LFloat.zero; // 距离

        public Plane(){ }

        public Plane(LVector3 normal, LFloat d){
            this.normal.set(normal).nor();
            this.d = d;
        }

        public Plane(LVector3 normal, LVector3 point){
            this.normal.set(normal).nor();
            this.d = -this.normal.dot(point);
        }

        public Plane(LVector3 point1, LVector3 point2, LVector3 point3){
            set(point1, point2, point3);
        }

        public void set(LVector3 point1, LVector3 point2, LVector3 point3){
            normal = (point1).sub(point2).cross(point2.x - point3.x, point2.y - point3.y, point2.z - point3.z).nor();
            d = -point1.dot(normal);
        }



        public LFloat distance(LVector3 point){
            return normal.dot(point) + d;
        }

        public PlaneSide testPoint(LVector3 point){
            LFloat dist = normal.dot(point) + d;

            if (dist == 0)
                return PlaneSide.OnPlane;
            else if (dist < 0)
                return PlaneSide.Back;
            else
                return PlaneSide.Front;
        }


        public PlaneSide testPoint(LFloat x, LFloat y, LFloat z){
            LFloat dist = normal.dot(x, y, z) + d;

            if (dist == 0)
                return PlaneSide.OnPlane;
            else if (dist < 0)
                return PlaneSide.Back;
            else
                return PlaneSide.Front;
        }


        public bool isFrontFacing(LVector3 direction){
            LFloat dot = normal.dot(direction);
            return dot <= 0;
        }

        /** @return The normal */
        public LVector3 getNormal(){
            return normal;
        }

        /** @return The distance to the origin */
        public LFloat getD(){
            return d;
        }


        public void set(LVector3 point, LVector3 normal){
            this.normal.set(normal);
            d = -point.dot(normal);
        }

        public void set(LFloat pointX, LFloat pointY, LFloat pointZ, LFloat norX, LFloat norY, LFloat norZ){
            this.normal.set(norX, norY, norZ);
            d = -(pointX * norX + pointY * norY + pointZ * norZ);
        }


        public void set(Plane plane){
            this.normal.set(plane.normal);
            this.d = plane.d;
        }

        public override String ToString(){
            return normal.ToString() + ", " + d;
        }
    }
}