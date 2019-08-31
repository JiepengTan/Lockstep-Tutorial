using Lockstep.Math;

namespace Lockstep.Game {
    public static class TankUtil {
        public static LFloat TANK_HALF_LEN = new LFloat(1);
        public static LFloat FORWARD_HEAD_DIST = new LFloat(true, 20);
        public static LFloat SNAP_DIST = new LFloat(true, 400);

        public static LVector2 GetHeadPos(LVector2 pos, EDir dir){
            var dirVec = DirUtil.GetDirLVec(dir);
            var fTargetHead = pos + (TANK_HALF_LEN + FORWARD_HEAD_DIST) * dirVec;
            return fTargetHead;
        }
        public static LVector2 GetHeadPos(LVector2 pos, EDir dir, LFloat len){
            var dirVec = DirUtil.GetDirLVec(dir);
            var fTargetHead = pos + (TANK_HALF_LEN + len) * dirVec;
            return fTargetHead;
        }
    }
}