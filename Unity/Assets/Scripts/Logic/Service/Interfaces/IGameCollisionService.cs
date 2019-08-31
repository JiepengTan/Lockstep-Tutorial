using Lockstep.Math;

namespace Lockstep.Game {
    public interface IGameCollisionService : IService {
        LFloat TANK_BORDER_SIZE { get; }
        bool HasColliderWithBorder(EDir dir, LVector2 fTargetHead);
        bool HasColliderWithBorder(EDir dir, LVector2 fTargetHead, LFloat size);

        bool HasCollider(LVector2 pos);

        bool IsOutOfBound(LVector2 fPos, LVector2Int min, LVector2Int max);

        LFloat GetMaxMoveDist(EDir dir, LVector2 fHeadPos, LVector2 fTargetHeadPos);

        LFloat GetMaxMoveDist(EDir dir, LVector2 fHeadPos, LVector2 fTargetHeadPos, LFloat borderSize);
    }
}