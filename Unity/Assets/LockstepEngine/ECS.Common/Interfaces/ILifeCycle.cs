namespace Lockstep.Game {
    public interface ILifeCycle {
        void DoAwake(IServiceContainer services);
        void DoStart();
        void DoUpdate(int deltaTimeMs);
        void DoFixedUpdate();
        void DoDestroy();
        void OnApplicationQuit();
    }
}