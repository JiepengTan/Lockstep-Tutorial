using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Math {
    public static partial class LMathExtension {
        public static LVector2 ToLVector2(this LVector2Int vec){
            return new LVector2(true, vec.x * LFloat.Precision, vec.y * LFloat.Precision);
        }

        public static LVector3 ToLVector3(this LVector3Int vec){
            return new LVector3(true, vec.x * LFloat.Precision, vec.y * LFloat.Precision, vec.z * LFloat.Precision);
        }

        public static LVector2Int ToLVector2Int(this LVector2 vec){
            return new LVector2Int(vec.x.ToInt(), vec.y.ToInt());
        }

        public static LVector3Int ToLVector3Int(this LVector3 vec){
            return new LVector3Int(vec.x.ToInt(), vec.y.ToInt(), vec.z.ToInt());
        }
    }

    public static partial class LMathExtension {
        public static LFloat ToLFloat(this float v){
            return LMath.ToLFloat(v);
        }

        public static LFloat ToLFloat(this int v){
            return LMath.ToLFloat(v);
        }

        public static LFloat ToLFloat(this long v){
            return LMath.ToLFloat(v);
        }
    }

    public static partial class LMathExtension {
        public static LVector2Int Floor(this LVector2 vec){
            return new LVector2Int(LMath.FloorToInt(vec.x), LMath.FloorToInt(vec.y));
        }

        public static LVector3Int Floor(this LVector3 vec){
            return new LVector3Int(
                LMath.FloorToInt(vec.x),
                LMath.FloorToInt(vec.y),
                LMath.FloorToInt(vec.z)
            );
        }
    }

    public static partial class LMathExtension {
        public static LVector2 RightVec(this LVector2 vec){
            return new LVector2(true, vec._y, -vec._x);
        }

        public static LVector2 LeftVec(this LVector2 vec){
            return new LVector2(true, -vec._y, vec._x);
        }

        public static LVector2 BackVec(this LVector2 vec){
            return new LVector2(true, -vec._x, -vec._y);
        }
        public static LFloat ToDeg(this LVector2 vec){
            return CTransform2D.ToDeg(vec);
        }
        
        public static LFloat Abs(this LFloat val){
            return LMath.Abs(val);
        }
    }
}