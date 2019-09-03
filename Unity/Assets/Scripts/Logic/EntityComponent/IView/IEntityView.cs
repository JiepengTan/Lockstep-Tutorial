using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {

    public interface IEntityView : IView {
        void OnTakeDamage(int amount, LVector3 hitPoint);
        void OnDead();
        void OnRollbackDestroy();
    }
}