using System;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    [Serializable]
    public class Enemy : Entity {
        public CBrain brain = new CBrain();

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