using System;
using Lockstep.Game;

namespace LockstepTutorial {
    
    [Serializable]
    public partial class Component : BaseComponent {
        public Entity entity { get; private set; }
        public IGameStateService GameStateService => entity.GameStateService;

        public override void BindEntity(BaseEntity entity){
            base.BindEntity(entity);
            this.entity = (Entity) entity;
        }
    }
}