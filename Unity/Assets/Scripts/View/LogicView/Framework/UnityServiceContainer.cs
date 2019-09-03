using Lockstep.Game;



public class UnityServiceContainer : BaseGameServicesContainer {
    public UnityServiceContainer():base(){
        RegisterService(new UnityGameViewService());
    }
}