using Lockstep.Game;

public class PureServiceContainer : BaseGameServicesContainer {
    public PureServiceContainer():base(){
        RegisterService(new PureGameViewService());
    }
}