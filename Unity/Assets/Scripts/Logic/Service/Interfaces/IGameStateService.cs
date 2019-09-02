using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.Game;

namespace Lockstep.Game {
    public interface IGameStateService : IService ,IEntityService{
        //changed in the game
        LFloat RemainTime { get; set; }
        LFloat DeltaTime { get; set; }
        
        List<Enemy> GetEnemies();
        List<Player> GetPlayers();
        List<Spawner> GetSpawners();
        object GetEntity(int id);
        void CreateEnemy(int prefabId, LVector3 position);
    }
}