using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;

namespace Lockstep.Logic {
    public partial class BaseComponent : IComponent {
        public BaseEntity entity;
        public CTransform2D transform;
        public virtual void BindEntity(BaseEntity entity){
            this.entity = entity;
            transform = entity.transform;
        }

        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }
}