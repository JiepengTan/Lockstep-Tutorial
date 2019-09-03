
using Lockstep.Math;
using Lockstep.Game;

namespace Lockstep.Game {
    public class BaseSystem : BaseGameService {
        public bool enable = true;
        public virtual void DoUpdate(LFloat deltaTime){ }
    }

    public abstract class BaseGameService : BaseService,IBaseGameManager {
        
        protected INetworkService _networkService;
        protected ISimulatorService SimulatorServiceService;
        protected IUIService _uiService;
        
        protected IGameStateService _gameStateService;
        protected IGameEffectService _gameEffectService;
        protected IGameAudioService _gameAudioService;
        protected IGameConfigService _gameConfigService;
        protected IGameViewService _gameViewService;
        protected IGameResourceService _gameResourceService;
        
        public override void InitReference( IServiceContainer serviceContainer,IManagerContainer mgrContainer){
            base.InitReference(serviceContainer,mgrContainer);
            
            _networkService = serviceContainer.GetService<INetworkService>();
            SimulatorServiceService = serviceContainer.GetService<ISimulatorService>();
            _uiService = serviceContainer.GetService<IUIService>();
            
            _gameEffectService = serviceContainer.GetService<IGameEffectService>();
            _gameAudioService = serviceContainer.GetService<IGameAudioService>();
            _gameStateService = serviceContainer.GetService<IGameStateService>();
            _gameConfigService = serviceContainer.GetService<IGameConfigService>();
            _gameViewService = serviceContainer.GetService<IGameViewService>();
            _gameResourceService = serviceContainer.GetService<IGameResourceService>();
        }
    }
}