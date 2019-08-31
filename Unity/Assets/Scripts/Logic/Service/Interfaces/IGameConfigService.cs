using System.Collections.Generic;
using Lockstep.ECS;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IGameConfigService : IService {
        List<BaseEntitySetter> enemyPrefabs { get; }
        List<BaseEntitySetter> playerPrefabs { get; }
        List<BaseEntitySetter> bulletPrefabs { get; }
        List<BaseEntitySetter> itemPrefabs { get; }
        List<BaseEntitySetter> CampPrefabs { get; }
        short BornPrefabAssetId { get; }
        short DiedPrefabAssetId { get; }
        float bornEnemyInterval { get; }
        int MAX_ENEMY_COUNT { get; }
        int initEnemyCount { get; }

        int MaxPlayerCount { get; }
        LVector2 TankBornOffset { get; }
        LFloat TankBornDelay { get; }
        LFloat DeltaTime { get; }
        string RelPath { set; }
    }
}