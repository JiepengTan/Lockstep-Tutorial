using Lockstep.Logic;
using Lockstep.Math;

namespace LockstepTutorial {
    public interface IPlayerView :IView {
        void TakeDamage(int amount, LVector3 hitPoint);
        void OnDead();
        void Animating(bool isIdle);
    }
}