using Lockstep.Math;

namespace Lockstep.Game {
    public interface IRandomService : IService {
        LFloat value { get; }
        uint Next();
        uint Next(uint max);
        int Next(int max);
        uint Range(uint min, uint max);
        int Range(int min, int max);
        LFloat Range(LFloat min, LFloat max);
    }
}