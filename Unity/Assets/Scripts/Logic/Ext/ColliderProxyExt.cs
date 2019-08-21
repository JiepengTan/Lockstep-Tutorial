using Lockstep.Logic;
using LockstepTutorial;

namespace Lockstep.Collision2D {
    public partial class ColliderProxy {
        public BaseActor Entity => (BaseActor) EntityObject;
    }
}