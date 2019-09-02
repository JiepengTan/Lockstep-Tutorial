using Lockstep.Game;

namespace Lockstep.Game {
    public interface IGameResourceService : IService {
        object LoadPrefab(int id);
    }
}