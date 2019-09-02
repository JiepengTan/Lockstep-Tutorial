using System.Collections.Generic;
using Lockstep.ECS;
using Lockstep.Math;
using Lockstep.Game;
using NetMsg.Common;

namespace Lockstep.Game {
    public interface IGameConfigService : IService {
        EnemyConfig GetEnemyConfig(int id);

        PlayerConfig GetPlayerConfig(int id);

        CollisionConfig CollisionConfig { get; }
        SpawnerConfig SpawnerConfig { get; }

        string RecorderFilePath { get; }
        Msg_G2C_GameStartInfo ClientModeInfo{ get; }
    }
}