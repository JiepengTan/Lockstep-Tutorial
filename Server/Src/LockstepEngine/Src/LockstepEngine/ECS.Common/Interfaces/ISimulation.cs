namespace Lockstep.Game {
    public interface ISimulation : IService {
        void RunVideo();
        void JumpTo(int tick);
    }
}