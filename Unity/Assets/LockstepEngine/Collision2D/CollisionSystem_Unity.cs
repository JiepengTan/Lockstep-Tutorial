#if UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
using Random = System.Random;

namespace Lockstep.Collision2D {
    public partial class CollisionSystem {
        public static ColliderPrefab CreateColliderPrefab(GameObject fab,ColliderData data){
            Debug.Trace("CreateColliderPrefab " + fab.name);
            CBaseShape collider = null;
            if (data == null) {
                Debug.LogError(fab.name + " Miss ColliderDataMono ");
                return null;
            }

            if (data.radius > 0) {
                //circle
                collider = new CCircle(data.radius);
            }
            else {
                //obb
                collider = new COBB(data.size, data.deg);
            }
            Debug.Trace($"{fab.name} !!!CreateCollider  deg: {data.deg} up:{data.size} radius:{data.radius}");
            var colFab = new ColliderPrefab();
            colFab.parts.Add(new ColliderPart() {
                transform = new CTransform2D(LVector2.zero),
                collider = collider
            });
            return colFab;
        }
    }
}
#endif