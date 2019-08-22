using System;
using Lockstep.Math;

namespace LockstepTutorial {
    [Serializable]
    public class Spawner {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;
        public Action<int, LVector3> OnSpawnEvent;

        private LFloat timer;
        public virtual void DoStart(){
            timer = spawnTime;
        }

        public virtual void DoUpdate(LFloat deltaTime){
            timer += deltaTime;
            if (timer > spawnTime) {
                timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn(){
            OnSpawnEvent(prefabId, spawnPoint);
        }
    }
}