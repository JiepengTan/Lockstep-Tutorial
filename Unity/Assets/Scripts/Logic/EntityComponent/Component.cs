using System;
using Lockstep.Game;

namespace Lockstep.Game {
    
    [Serializable]
    [NoBackup]
    public partial class Component : BaseComponent {
        public Entity entity =>(Entity) baseEntity;
        public IGameStateService GameStateService => entity.GameStateService;
        public IDebugService DebugService => entity.DebugService;

    }
}