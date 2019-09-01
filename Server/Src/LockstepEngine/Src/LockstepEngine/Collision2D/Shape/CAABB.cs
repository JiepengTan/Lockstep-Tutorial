using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

namespace Lockstep.Collision2D {
    public class CAABB : CCircle {
        public override int TypeId => (int) EShape2D.AABB;
        /// <summary> Half size of BoundBox</summary>
        public LVector2 size;

        public CAABB() : base(){ }

        public CAABB(LVector2 size){
            this.size = size;
            radius = size.magnitude;
        }
        public override string ToString(){
            return $"(radius:{radius} deg:{radius} up:{size})";
        }
    }
}