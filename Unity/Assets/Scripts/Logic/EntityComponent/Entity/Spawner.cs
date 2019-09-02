using System;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    public partial class Spawner : BaseEntity {
        public SpawnerInfo info = new SpawnerInfo();
        [NoBackup] public Action<int, LVector3> OnSpawnEvent;

        [Backup] private LFloat _timer;

        public override void DoStart(){
            _timer = info.spawnTime;
        }

        public override void DoUpdate(LFloat deltaTime){
            _timer += deltaTime;
            if (_timer > info.spawnTime) {
                _timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn(){
            OnSpawnEvent(info.prefabId, info.spawnPoint);
        }
    }
}