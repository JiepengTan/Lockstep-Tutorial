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
        public static ColliderPrefab CreateColliderPrefab(GameObject fab){
            Debug.Trace("CreateColliderPrefab " + fab.name);
#if false
            Collider unityCollider = null;
            var colliders = fab.GetComponents<Collider>();
            foreach (var col in colliders) {
                if (col.isTrigger) {
                    unityCollider = col;
                    break;
                }
            }

            if (unityCollider == null) {
                foreach (var col in colliders) {
                    unityCollider = col;
                    break;
                }
            }

            if (unityCollider == null) return null;
            CBaseShape collider = null;
            if (unityCollider is BoxCollider boxCol) {
                collider = new COBB(boxCol.size.ToLVector2XZ(), LFloat.zero);
            }

            if (unityCollider is SphereCollider cirCol) {
                collider = new CCircle(cirCol.radius.ToLFloat());
            }

            if (unityCollider is CapsuleCollider capCol) {
                collider = new CCircle(capCol.radius.ToLFloat());
            }

            if (collider is COBB tObb) {
                Debug.LogTrace($"{fab.name} CreateCollider OBB deg: {tObb.deg} up:{tObb.up} radius:{tObb.radius}");
            }
            if (collider is CCircle tCircle) {
                Debug.LogTrace($"{fab.name} CreateCollider Circle deg: radius:{tCircle.radius}");
            }
#else
            CBaseShape collider = null;
            var data = fab.GetComponent<ColliderDataMono>()?.colliderData;
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
#endif
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