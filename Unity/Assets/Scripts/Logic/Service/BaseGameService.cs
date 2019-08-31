
using Lockstep.Math;

namespace Lockstep.Game {
    public class BaseLogicManager : BaseGameService {
        public virtual void DoUpdate(LFloat deltaTime){ }
    }

    public abstract class BaseGameService : BaseService,IBaseGameManager {
        public void AssignReference( IServiceContainer serviceContainer,
            IManagerContainer mgrContainer
        ){
            InitReference(serviceContainer);
            InitMgrReference(serviceContainer);
        }
        
        protected INetworkService _networkService;
        protected ISimulation _simulationService;
        protected IUIService _uiService;
        public void InitMgrReference(IServiceContainer serviceContainer){
            
            _networkService = serviceContainer.GetService<INetworkService>();
            _simulationService = serviceContainer.GetService<ISimulation>();
            _uiService = serviceContainer.GetService<IUIService>();
        }
        
        
        protected IGameConstStateService _gameConstStateService;
        protected IGameStateService _gameStateService;
        protected IGameEffectService _gameEffectService;
        protected IGameAudioService _gameAudioService;
        protected IGameUnitService _gameUnitService;
        protected IGameCollisionService _gameCollisionService;
        protected IGameConfigService _gameConfigService;
        

        public override void InitReference(IServiceContainer serviceContainer){
            base.InitReference(serviceContainer);
            _gameEffectService = serviceContainer.GetService<IGameEffectService>();
            _gameAudioService = serviceContainer.GetService<IGameAudioService>();
            _gameUnitService = serviceContainer.GetService<IGameUnitService>();
            _gameConstStateService = serviceContainer.GetService<IGameConstStateService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _gameCollisionService = serviceContainer.GetService<IGameCollisionService>();
            _gameConfigService = serviceContainer.GetService<IGameConfigService>();
        }
    }
}