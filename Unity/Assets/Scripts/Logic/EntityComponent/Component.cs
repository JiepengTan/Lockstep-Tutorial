using System;
using Lockstep.Logic;

namespace LockstepTutorial {
    
    [Serializable]
    public class Component : BaseComponent {
        public Entity entity { get; private set; }

        public override void BindEntity(BaseEntity entity){
            base.BindEntity(entity);
            this.entity = (Entity) entity;
        }
    }
}