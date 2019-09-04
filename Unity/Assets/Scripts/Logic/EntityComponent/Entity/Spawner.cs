using System;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {    
    [Serializable]
    public partial class Spawner : BaseEntity {
        public SpawnerInfo Info = new SpawnerInfo();
        public LFloat Timer;

        public override void DoStart(){
            Timer = Info.spawnTime;
        }

        public override void DoUpdate(LFloat deltaTime){
            Timer += deltaTime;
            if (Timer > Info.spawnTime) {
                Timer = LFloat.zero;
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