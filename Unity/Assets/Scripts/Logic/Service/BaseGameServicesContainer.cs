using Lockstep.Game;
using LockstepTutorial;

public class BaseGameServicesContainer : ServiceContainer {
    public BaseGameServicesContainer(){
        RegisterService(new ConstStateService());
        RegisterService(new GameResourceService());
        RegisterService(new GameEntityService());
        RegisterService(new GameStateService());
        RegisterService(new GameConfigService());
    }
}