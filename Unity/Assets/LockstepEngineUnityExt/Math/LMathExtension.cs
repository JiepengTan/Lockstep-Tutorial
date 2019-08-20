
using Lockstep.UnsafeCollision2D;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif
using Lockstep.Math;

namespace Lockstep.Math {
#if UNITY_5_3_OR_NEWER
    public static partial class LMathExtension {
        public static LVector2 ToLVector2(this Vector2Int vec){
            return new LVector2(true,vec.x * LFloat.Precision, vec.y * LFloat.Precision);
        }       
     
        public static LVector3 ToLVector3(this Vector3Int vec){
            return new LVector3(true,vec.x * LFloat.Precision, vec.y * LFloat.Precision, vec.z * LFloat.Precision);
        }
     
        public static LVector2Int ToLVector2Int(this Vector2Int vec){
            return new LVector2Int(vec.x, vec.y);
        }

        public static LVector3Int ToLVector3Int(this Vector3Int vec){
            return new LVector3Int(vec.x, vec.y, vec.z);
        }
        public static Vector2Int ToVector2Int(this LVector2Int vec){
            return new Vector2Int(vec.x, vec.y);
        }

        public static Vector3Int ToVector3Int(this LVector3Int vec){
            return new Vector3Int(vec.x, vec.y, vec.z);
        }
        public static LVector2 ToLVector2(this Vector2 vec){
            return new LVector2(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.y));
        }

        public static LVector3 ToLVector3(this Vector3 vec){
            return new LVector3(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.y),
                LMath.ToLFloat(vec.z));
        }
        public static LVector2 ToLVector2XZ(this Vector3 vec){
            return new LVector2(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.z));
        }
        public static Vector2 ToVector2(this LVector2 vec){
            return new Vector2(vec.x.ToFloat(), vec.y.ToFloat());
        }
        public static Vector3 ToVector3(this LVector2 vec){
            return new Vector3(vec.x.ToFloat(), vec.y.ToFloat(),0);
        }
        public static Vector3 ToVector3XZ(this LVector2 vec,LFloat y){
            return new Vector3(vec.x.ToFloat(), y.ToFloat(),vec.y.ToFloat());
        }
        public static Vector3 ToVector3XZ(this LVector2 vec){
            return new Vector3(vec.x.ToFloat(), 0,vec.y.ToFloat());
        }
        public static Vector3 ToVector3(this LVector3 vec){
            return new Vector3(vec.x.ToFloat(), vec.y.ToFloat(), vec.z.ToFloat());
        }
        public static Rect ToRect(this LRect vec){
            return new Rect(vec.position.ToVector2(),vec.size.ToVector2());
        }
    }
#endif
}