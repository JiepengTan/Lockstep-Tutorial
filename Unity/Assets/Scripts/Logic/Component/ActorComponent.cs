using Lockstep.Logic;

namespace LockstepTutorial {
    public class ActorComponent : BaseComponent {
        public BaseActor baseActor;

        public override void BindEntity(BaseEntity entity){
            base.BindEntity(entity);
            baseActor = (BaseActor) entity;
        }
    }
}