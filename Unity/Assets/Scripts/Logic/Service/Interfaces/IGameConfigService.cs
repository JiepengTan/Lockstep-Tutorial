using System.Collections.Generic;
using Lockstep.ECS;
using Lockstep.Math;
using LockstepTutorial;

namespace Lockstep.Game {
    public interface IGameConfigService : IService {
        EnemyConfig GetEnemyConfig(int id);

        PlayerConfig GetPlayerConfig(int id);

        CollisionConfig CollisionConfig { get; }
        SpawnerConfig SpawnerConfig { get; }
    }
}