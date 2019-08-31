using Lockstep.Math;

namespace Lockstep.Game {

    public static class DirUtil {
        public static LVector2 GetDirLVec(EDir dir){
            switch (dir) {
                case EDir.Up: return LVector2.up;
                case EDir.Right: return LVector2.right;
                case EDir.Down: return LVector2.down;
                case EDir.Left: return LVector2.left;
            }

            return LVector2.up;
        }
        public static int GetDirDeg(EDir dir){
            return ((int) dir) * 90;
        }

        public static LVector2 GetBorderDir(EDir dir){
            var isUpDown = (int) (dir) % 2 == 0;
            var borderDir = LVector2.up;
            if (isUpDown) {
                borderDir = LVector2.right;
            }

            return borderDir;
        }
    
    }
}