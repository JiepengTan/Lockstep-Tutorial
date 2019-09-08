namespace Lockstep.Game {
    public interface ISimulatorService : IService {
        void RunVideo();
        void JumpTo(int tick);
    }

    public interface IDebugService : IService{
        void Trace(string msg, bool isNewLine = false, bool isNeedLogTrace = false);
    }
}