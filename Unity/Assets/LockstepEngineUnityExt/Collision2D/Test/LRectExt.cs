using UnityEngine;

namespace Lockstep.Collision2D {
    public static partial class LRectExt {
        public static Vector2 ToVector2XZ(this Vector3 vec){
            return new Vector2(vec.x, vec.z);
        }

        public static Vector3 ToVector3(this Vector2 vec, int y = 1){
            return new Vector3(vec.x, y, vec.y);
        }

    }
}