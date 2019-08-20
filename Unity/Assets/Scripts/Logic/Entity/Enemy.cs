using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class Enemy : BaseEntity {
        public IEnemyView eventHandler;
        public BaseEntity target;
    }
}