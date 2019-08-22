using System;
using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;

namespace Lockstep.Logic {
    [Serializable]
    public partial class BaseComponent : IComponent {
        public BaseEntity baseEntity { get; private set; }
        public CTransform2D transform { get; private set; }

        public virtual void BindEntity(BaseEntity entity){
            this.baseEntity = entity;
            transform = entity.transform;
        }

        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }
}