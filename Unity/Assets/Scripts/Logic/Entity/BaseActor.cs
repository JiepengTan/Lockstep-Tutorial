using System;
using Lockstep.Logging;
using Lockstep.Logic;
using Lockstep.Math;

namespace LockstepTutorial {
    public partial class BaseActor : BaseEntity {
        public IActorView actorView;
        public LFloat moveSpd = 5;
        public LFloat turnSpd = 360;
        public int curHealth;
        public int maxHealth = 100;
        public bool isDead => curHealth <= 0;

        public BaseActor(){
            curHealth = maxHealth;
        }

        public virtual void TakeDamage(int amount, LVector3 hitPoint){
            if (isDead) return;
            curHealth -= amount;
            actorView?.OnTakeDamage(amount, hitPoint);
            OnTakeDamage(amount, hitPoint);
            if (isDead) {
                actorView?.OnDead();
                OnDead();
            }
        }

        protected virtual void OnTakeDamage(int amount, LVector3 hitPoint){}

        protected virtual void OnDead(){}
    }
}