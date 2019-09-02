using System;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    public partial class Spawner : IEntity {
        public SpawnerInfo info;
        public Action<int, LVector3> OnSpawnEvent;

        private LFloat timer;

        public virtual void DoStart(){
            timer = info.spawnTime;
        }

        public virtual void DoUpdate(LFloat deltaTime){
            timer += deltaTime;
            if (timer > info.spawnTime) {
                timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn(){
            OnSpawnEvent(info.prefabId, info.spawnPoint);
        }
    }
}