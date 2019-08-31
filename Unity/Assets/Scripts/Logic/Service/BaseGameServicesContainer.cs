using Lockstep.Game;
using LockstepTutorial;

public class BaseGameServicesContainer : ServiceContainer {
    public BaseGameServicesContainer(){
        RegisterService(new ConstStateService());
        RegisterService(new GameConstStateService());
        RegisterService(new GameStateService());
        RegisterService(new ResourceGameService());
    }
}