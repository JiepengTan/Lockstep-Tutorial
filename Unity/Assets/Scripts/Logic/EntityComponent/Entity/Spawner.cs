using System;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {    
    [Serializable]
    public partial class Spawner : BaseEntity {
        public SpawnerInfo Info = new SpawnerInfo();
        [Backup] private LFloat _timer;

        public override void DoStart(){
            _timer = Info.spawnTime;
        }

        public override void DoUpdate(LFloat deltaTime){
            _timer += deltaTime;
            if (_timer > Info.spawnTime) {
                _timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn(){
            if (GameStateService.CurEnemyCount >= GameStateService.MaxEnemyCount) {
                return;
            }

            GameStateService.CurEnemyCount++;
            GameStateService.CreateEntity<Enemy>(Info.prefabId, Info.spawnPoint);
        }
    }
}