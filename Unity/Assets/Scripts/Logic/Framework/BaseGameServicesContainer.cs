using Lockstep.Game;
using Lockstep.Game;

public class BaseGameServicesContainer : ServiceContainer {
    public BaseGameServicesContainer(){
        RegisterService(new RandomService());
        RegisterService(new CommonStateService());
        RegisterService(new ConstStateService());
        RegisterService(new SimulatorService());
        RegisterService(new NetworkService());
        RegisterService(new IdService());
        RegisterService(new GameResourceService());
        
        RegisterService(new GameStateService());
        RegisterService(new GameConfigService());
        RegisterService(new GameInputService());
    }
}