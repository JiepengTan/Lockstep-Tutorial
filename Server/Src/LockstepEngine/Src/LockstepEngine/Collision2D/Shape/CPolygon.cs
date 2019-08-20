using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

namespace Lockstep.Collision2D {
    public class CPolygon : CCircle {
        public override int TypeId => (int) EShape2D.Polygon;
        public int vertexCount;
        public LFloat deg;
        public LVector2[] vertexes;
    }
}