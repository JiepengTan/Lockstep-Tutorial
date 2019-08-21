using UnityEngine;

namespace Lockstep.Logic {
    public static class EntityUnityExt {
        public static Transform GetUnityTransform(this BaseEntity value){
            return value.engineTransform as Transform;
        }
    }
}