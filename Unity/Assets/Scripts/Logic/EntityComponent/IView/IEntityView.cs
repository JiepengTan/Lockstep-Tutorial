using Lockstep.Logic;
using Lockstep.Math;

namespace LockstepTutorial {
    public interface IEntityView : IView {
        void OnTakeDamage(int amount, LVector3 hitPoint);
        void OnDead();
    }
}