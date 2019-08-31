using Lockstep.Game;
using LockstepTutorial;

namespace Lockstep.Collision2D {
    public partial class ColliderProxy {
        public Entity Entity => (Entity) EntityObject;
    }
}