using Lockstep.Logic;

namespace LockstepTutorial {
    public abstract partial class BasePlayerComponent : BaseComponent {
        public Player player;
        public PlayerInput input => player.InputAgent;

        public override void BindEntity(BaseEntity entity){
            base.BindEntity(entity);
            player = (Player) entity;
        }
    }
}