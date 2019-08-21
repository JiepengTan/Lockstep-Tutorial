using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class Enemy : BaseActor {
        public CBrain brain = new CBrain();
        public int localId;

        public Enemy(){
            moveSpd = 2;
            turnSpd = 150;
            RegisterComponent(brain);
        }

        protected override void OnDead(){
            EnemyManager.Instance.RemoveEnemy(this);
        }
    }
}