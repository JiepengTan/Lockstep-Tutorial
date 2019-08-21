using Lockstep.Logic;
using Lockstep.Math;

namespace LockstepTutorial {
    public interface IActorView : IView {
        void OnTakeDamage(int amount, LVector3 hitPoint);
        void OnDead();
    }
}