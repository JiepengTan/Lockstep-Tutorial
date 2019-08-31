using System;
using Lockstep.Math;

namespace LockstepTutorial {


    [Serializable]
    public class SpawnerInfo {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;
    }
}