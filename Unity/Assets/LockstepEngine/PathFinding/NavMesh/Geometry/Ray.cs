using System;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public class Ray {
        private static long serialVersionUID = -620692054835390878L;
        public LVector3 origin = new LVector3(); // 
        public LVector3 direction = new LVector3(); // 

        public Ray(){ }

        public Ray(LVector3 origin, LVector3 direction){
            this.origin.set(origin);
            this.direction.set(direction).nor();
        }

        /** @return a copy of this ray. */
        public Ray cpy(){
            return new Ray(this.origin, this.direction);
        }


        public LVector3 getEndPoint(LVector3 _out, LFloat distance){
            return _out.set(direction).scl(distance).Add(origin);
        }

        static LVector3 tmp = new LVector3();


        /** {@inheritDoc} */
        public String toString(){
            return "ray [" + origin + ":" + direction + "]";
        }


        public Ray set(LVector3 origin, LVector3 direction){
            this.origin.set(origin);
            this.direction.set(direction);
            return this;
        }

        public Ray set(LFloat x, LFloat y, LFloat z, LFloat dx, LFloat dy, LFloat dz){
            this.origin.set(x, y, z);
            this.direction.set(dx, dy, dz);
            return this;
        }


        public Ray set(Ray ray){
            this.origin.set(ray.origin);
            this.direction.set(ray.direction);
            return this;
        }
    }
}