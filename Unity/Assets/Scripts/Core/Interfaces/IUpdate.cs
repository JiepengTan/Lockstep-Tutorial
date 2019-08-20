using Lockstep.Math;

namespace Lockstep.Logic {
    public interface IUpdate {
        void DoUpdate(LFloat deltaTime);
    }
}