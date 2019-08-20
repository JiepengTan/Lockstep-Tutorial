namespace Lockstep.Game {
    public interface IECSFacadeService :IService {
        IContexts CreateContexts();
    }
}