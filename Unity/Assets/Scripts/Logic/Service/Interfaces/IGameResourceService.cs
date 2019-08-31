using Lockstep.Game;

namespace LockstepTutorial {
    public interface IGameResourceService : IService {
        object LoadPrefab(int id);
    }
}