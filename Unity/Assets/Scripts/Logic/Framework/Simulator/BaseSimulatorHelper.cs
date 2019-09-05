namespace Lockstep.Game {
    public class BaseSimulatorHelper {
        public int Tick => _world.Tick;
        protected World _world;
        protected IServiceContainer _serviceContainer;

        public BaseSimulatorHelper(IServiceContainer serviceContainer, World world){
            this._world = world;
            this._serviceContainer = serviceContainer;
        }
    }
}