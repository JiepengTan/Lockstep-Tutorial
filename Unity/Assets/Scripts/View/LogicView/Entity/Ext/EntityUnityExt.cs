using UnityEngine;

namespace Lockstep.Game {
    public static class EntityUnityExt {
        public static Transform GetUnityTransform(this BaseEntity value){
            return value.engineTransform as Transform;
        }
    }
}