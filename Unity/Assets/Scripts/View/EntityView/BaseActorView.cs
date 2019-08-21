using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial {
    public abstract class BaseActorView : MonoBehaviour, IActorView {
        public UIFloatBar uiFloatBar;
        public BaseActor baseActor;
        protected bool isDead => baseActor?.isDead ?? true;

        public virtual void BindEntity(BaseEntity entity){
            baseActor = entity as BaseActor;
            baseActor.actorView = this;
            uiFloatBar = FloatBarManager.CreateFloatBar(transform, baseActor.curHealth, baseActor.maxHealth);
            transform.position = baseActor.transform.Pos3.ToVector3();
        }

        public virtual void OnTakeDamage(int amount, LVector3 hitPoint){
            uiFloatBar.UpdateHp(baseActor.curHealth, baseActor.maxHealth);
            FloatTextManager.CreateFloatText(hitPoint.ToVector3(), -amount);
        }

        public virtual void OnDead(){
            if (uiFloatBar != null) FloatBarManager.DestroyText(uiFloatBar);
            GameObject.Destroy(gameObject);
        }

        private void Update(){
            var pos = baseActor.transform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = baseActor.transform.deg.ToFloat();
            //deg = Mathf.Lerp(transform.rotation.eulerAngles.y, deg, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
        }
    }
}