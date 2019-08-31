using System;
using System.Linq;
using System.Reflection;
using Lockstep.Math;
using Lockstep.Util;
using LockstepTutorial;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    [Serializable]
    public class Launcher : ILifeCycle {
        private int _curTick;

        public int CurTick {
            get => _curTick;
            set {
                _curTick = value;
                _timeMachineContainer.CurTick = value;
            }
        }

        public static Launcher Instance { get; private set; }

        private ServiceContainer _serviceContainer;
        private ManagerContainer _mgrContainer;
        private TimeMachineContainer _timeMachineContainer;
        private IEventRegisterService _registerService;
        public MainManager _mainManager = new MainManager();

        public EPureModeType RunMode = EPureModeType.Unity; //纯净模式  不含Unity 相关的代码
        public object transform;

        public void DoAwake(IServiceContainer services){
            if (Instance != null) {
                Debug.LogError("LifeCycle Error: Awake more than once!!");
                return;
            }

            Instance = this;
            _serviceContainer = services as ServiceContainer;
            _registerService = new EventRegisterService();
            _mgrContainer = new ManagerContainer();
            _timeMachineContainer = new TimeMachineContainer();

            //AutoCreateManagers;
            var svcs = _serviceContainer.GetAllServices();
            foreach (var service in svcs) {
                _timeMachineContainer.RegisterTimeMachine(service as ITimeMachine);
                if (service is BaseService baseService) {
                    _mgrContainer.RegisterManager(baseService);
                }
            }

            _serviceContainer.RegisterService(_timeMachineContainer);
            _serviceContainer.RegisterService(_registerService);
        }


        public void DoStart(){
            _mainManager.InitReference(_serviceContainer, _mgrContainer);
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.InitReference(_serviceContainer, _mgrContainer);
            }

            //bind events
            foreach (var mgr in _mgrContainer.AllMgrs) {
                _registerService.RegisterEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length,
                    EventHelper.AddListener, mgr);
            }

            _mainManager.DoAwake(_serviceContainer);
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoAwake(_serviceContainer);
            }

            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoStart();
            }

            _mainManager.DoStart();
        }

        public void DoUpdate(float deltaTime){
            _mainManager.DoUpdate(deltaTime);
        }

        public void DoDestroy(){
            if (Instance == null) return;
            foreach (var mgr in _mgrContainer.AllMgrs) {
                mgr.DoDestroy();
            }

            _mainManager.DoDestroy();
            Instance = null;
        }

        public void OnApplicationQuit(){
            DoDestroy();
        }
    }
}