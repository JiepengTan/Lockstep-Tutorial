namespace Lockstep.Game {

    public class ServiceReferenceHolder {
        protected IServiceContainer _serviceContainer;
        protected IECSFacadeService _ecsFacadeService;
        
        
        protected IRandomService _randomService;
        protected ITimeMachineService _timeMachineService;
        protected IConstStateService _constStateService;
        protected IViewService _viewService;
        protected IAudioService _audioService;
        protected IInputService _inputService;
        protected IMap2DService _map2DService;
        protected IResService _resService;
        protected IEffectService _effectService;
        protected IEventRegisterService _eventRegisterService;
        protected IIdService _idService;
        protected ICommonStateService _commonStateService;
        
        protected T GetService<T>() where T : IService{
            return _serviceContainer.GetService<T>();
        }

        public virtual void InitReference(IServiceContainer serviceContainer,IManagerContainer mgrContainer){
            _serviceContainer = serviceContainer;
            //通用Service
            _ecsFacadeService = serviceContainer.GetService<IECSFacadeService>();
            _randomService = serviceContainer.GetService<IRandomService>();
            _timeMachineService = serviceContainer.GetService<ITimeMachineService>();
            _constStateService = serviceContainer.GetService<IConstStateService>();
            _inputService = serviceContainer.GetService<IInputService>();
            _viewService = serviceContainer.GetService<IViewService>();
            _audioService = serviceContainer.GetService<IAudioService>();
            _map2DService = serviceContainer.GetService<IMap2DService>();
            _resService = serviceContainer.GetService<IResService>();
            _effectService = serviceContainer.GetService<IEffectService>();
            _eventRegisterService = serviceContainer.GetService<IEventRegisterService>();
            _idService = serviceContainer.GetService<IIdService>();
            _commonStateService = serviceContainer.GetService<ICommonStateService>();
            
        }
    }
   
}