using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

namespace Lockstep.Collision2D {
    public class COBB : CAABB {
        public override int TypeId => (int) EShape2D.OBB;
        public LFloat deg;
        public LVector2 up;

        public COBB(LVector2 size, LFloat deg) : base(size){
            this.deg = deg;
            SetDeg(deg);
        }

        public COBB(LVector2 size, LVector2 up) : base(size){
            SetUp(up);
        }

        //CCW 旋转角度
        public void Rotate(LFloat rdeg){
            deg += rdeg;
            if (deg > 360 || deg < -360) {
                deg = deg - (deg / 360 * 360);
            }

            SetDeg(deg);
        }

        public void SetUp(LVector2 up){
            this.up = up;
            this.deg = LMath.Atan2(-up.x, up.y);
        }

        public void SetDeg(LFloat rdeg){
            deg = rdeg;
            var rad = LMath.Deg2Rad * deg;
            var c = LMath.Cos(rad);
            var s = LMath.Sin(rad);
            up = new LVector2(-s, c);
        }
        public override string ToString(){
            return $"(radius:{radius} up:{size} deg:{radius} up:{up} )";
        }
    }
}