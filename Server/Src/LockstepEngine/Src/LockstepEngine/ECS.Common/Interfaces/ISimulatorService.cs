namespace Lockstep.Game {
    public interface ISimulatorService : IService {
        void RunVideo();
        void JumpTo(int tick);
    }
}