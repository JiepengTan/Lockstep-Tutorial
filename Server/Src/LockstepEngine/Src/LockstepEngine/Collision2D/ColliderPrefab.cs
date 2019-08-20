using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;

namespace Lockstep.Collision2D {
    public class ColliderPrefab {
        public List<ColliderPart> parts = new List<ColliderPart>();
        public CBaseShape collider => parts[0].collider;
        public CTransform2D transform => parts[0].transform;

        public LRect GetBounds(){
            //TODO
            var col = collider;
            var tran = transform;
            var type = (EShape2D) col.TypeId;
            switch (type) {
                case EShape2D.Circle: {
                    var radius = ((CCircle) col).radius;
                    return LRect.CreateRect(tran.pos, new LVector2(radius, radius));
                }
                case EShape2D.AABB: {
                    var halfSize = ((CAABB) col).size;
                    return LRect.CreateRect(tran.pos, halfSize);
                }
                case EShape2D.OBB: {
                    var radius = ((COBB) col).radius;
                    return LRect.CreateRect(tran.pos, new LVector2(radius, radius));
                }
            }

            Debug.LogError("No support type" + type);

            return new LRect();
        }

        public override string ToString(){
            return collider.ToString();
        }
    }
}