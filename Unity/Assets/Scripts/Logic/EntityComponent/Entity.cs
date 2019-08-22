using System;
using Lockstep.Logging;
using Lockstep.Logic;
using Lockstep.Math;

namespace LockstepTutorial {
    [Serializable]
    public partial class Entity : BaseEntity {
        public CAnimator animator = new CAnimator();
        public CSkillBox skillBox = new CSkillBox();
        public IEntityView EntityView;
        public LFloat moveSpd = 5;
        public LFloat turnSpd = 360;
        public int curHealth;
        public int maxHealth = 100;
        public int damage = 10;

        public bool isInvincible;
        public bool isFire;

        public bool isDead => curHealth <= 0;

        public Entity(){
            curHealth = maxHealth;
            RegisterComponent(animator);
        }

        public bool Fire(int idx = 0){
            return skillBox.Fire(idx);
        }

        public void StopSkill(int idx = -1){
            skillBox.ForceStop(idx);
        }

        public virtual void TakeDamage(int amount, LVector3 hitPoint){
            if (isDead) return;
            curHealth -= amount;
            EntityView?.OnTakeDamage(amount, hitPoint);
            OnTakeDamage(amount, hitPoint);
            if (isDead) {
                EntityView?.OnDead();
                OnDead();
            }
        }
        protected virtual void OnTakeDamage(int amount, LVector3 hitPoint){ }

        protected virtual void OnDead(){
            Debug.Log($"{EntityId} Dead");
        }
    }
}