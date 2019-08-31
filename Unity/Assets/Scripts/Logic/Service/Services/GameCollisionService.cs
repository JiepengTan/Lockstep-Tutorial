using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    public class GameCollisionService : BaseService, IGameCollisionService {
        LFloat _TANK_BORDER_SIZE = new LFloat(true, 900);
        public LFloat TANK_BORDER_SIZE => _TANK_BORDER_SIZE;

        public List<LVector3Int> DebugQueryCollider(EDir dir, LVector2 fTargetHead){
            return DebugQueryCollider(dir, fTargetHead, TANK_BORDER_SIZE);
        }

        public List<LVector3Int> DebugQueryCollider(EDir dir, LVector2 fTargetHead, LFloat size){
            var ret = new List<LVector3Int>();
            LVector2 borderDir = DirUtil.GetBorderDir(dir);
            var fBorder1 = fTargetHead + borderDir * size;
            var fBorder2 = fTargetHead - borderDir * size;
            var isColHead = HasCollider(fTargetHead);
            var isColBorder1 = HasCollider(fBorder1);
            var isColBorder2 = HasCollider(fBorder2);
            ret.Add(new LVector3Int(fTargetHead.Floor().x, fTargetHead.Floor().y, isColHead ? 1 : 0));
            ret.Add(new LVector3Int(fBorder1.Floor().x, fBorder1.Floor().y, isColBorder1 ? 1 : 0));
            ret.Add(new LVector3Int(fBorder2.Floor().x, fBorder2.Floor().y, isColBorder2 ? 1 : 0));
            return ret;
        }

        public bool HasColliderWithBorder(EDir dir, LVector2 fTargetHead){
            return HasColliderWithBorder(dir, fTargetHead, TANK_BORDER_SIZE);
        }

        public bool HasColliderWithBorder(EDir dir, LVector2 fTargetHead, LFloat size){
            LVector2 borderDir = DirUtil.GetBorderDir(dir);
            var fBorder1 = fTargetHead + borderDir * size;
            var fBorder2 = fTargetHead - borderDir * size;
            var isColHead = HasCollider(fTargetHead);
            var isColBorder1 = HasCollider(fBorder1);
            var isColBorder2 = HasCollider(fBorder2);
            return isColHead
                   || isColBorder1
                   || isColBorder2;
        }

        public bool HasCollider(LVector2 pos){
            var iPos = pos.Floor();
            var id = _map2DService.Pos2TileId(iPos, true);
            return id != 0;
        }

        public bool IsOutOfBound(LVector2 fpos, LVector2Int min, LVector2Int max){
            var pos = fpos.Floor();
            if (pos.x < min.x || pos.x > max.x
                              || pos.y < min.y || pos.y > max.y
            ) {
                return true;
            }

            return false;
        }

        public LFloat GetMaxMoveDist(EDir dir, LVector2 fHeadPos, LVector2 fTargetHeadPos){
            return GetMaxMoveDist(dir, fHeadPos, fTargetHeadPos, TANK_BORDER_SIZE);
        }

        public LFloat GetMaxMoveDist(EDir dir, LVector2 fHeadPos, LVector2 fTargetHeadPos, LFloat borderSize){
            var iTargetHeadPos =
                new LVector2Int(LMath.FloorToInt(fTargetHeadPos.x), LMath.FloorToInt(fTargetHeadPos.y));
            var hasCollider = HasColliderWithBorder(dir, fTargetHeadPos, borderSize);
            var maxMoveDist = LFloat.MaxValue;
            if (hasCollider) {
                switch (dir) {
                    case EDir.Up:
                        maxMoveDist = iTargetHeadPos.y - fHeadPos.y;
                        break;
                    case EDir.Right:
                        maxMoveDist = iTargetHeadPos.x - fHeadPos.x;
                        break;
                    case EDir.Down:
                        maxMoveDist = fHeadPos.y - iTargetHeadPos.y - 1;
                        break;
                    case EDir.Left:
                        maxMoveDist = fHeadPos.x - iTargetHeadPos.x - 1;
                        break;
                }
            }

            return maxMoveDist;
        }
    }
}