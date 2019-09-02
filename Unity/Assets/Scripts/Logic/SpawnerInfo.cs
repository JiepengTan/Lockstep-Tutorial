using System;
using Lockstep.Math;

namespace Lockstep.Game {
    [Serializable]
    public partial class SpawnerInfo : IComponent {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;
    }
}