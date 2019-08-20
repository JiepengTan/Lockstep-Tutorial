using Lockstep.Math;

namespace Lockstep.Game {
    public interface IEffectService : IService {
        void CreateEffect(int assetId, LVector2 pos);
        void DestroyEffect(object node);
    }
}