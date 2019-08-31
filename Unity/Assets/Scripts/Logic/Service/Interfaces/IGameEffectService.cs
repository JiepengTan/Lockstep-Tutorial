using Lockstep.Math;

namespace Lockstep.Game {
    public interface IGameEffectService : IService {
        void ShowDiedEffect(LVector2 pos);
        void ShowBornEffect(LVector2 pos);
    }
}