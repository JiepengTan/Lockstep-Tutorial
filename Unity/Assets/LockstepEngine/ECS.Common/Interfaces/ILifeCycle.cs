using Lockstep.Math;

namespace Lockstep.Game {
    public interface ILifeCycle {
        void DoAwake(IServiceContainer services);
        void DoStart();
        void DoDestroy();
        void OnApplicationQuit();
    }
}