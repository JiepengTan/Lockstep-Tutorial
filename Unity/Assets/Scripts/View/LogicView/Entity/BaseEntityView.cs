using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class BaseEntityView : MonoBehaviour, IEntityView {
        public BaseEntity baseEntity;
        public virtual void BindEntity(BaseEntity e){
            e.EntityView = this;
            this.baseEntity = e;
            transform.position = e.transform.Pos3.ToVector3();
        }
        
        public virtual void OnTakeDamage(int amount, LVector3 hitPoint){
            FloatTextManager.CreateFloatText(hitPoint.ToVector3(), -amount);
        }

        public virtual void OnDead(){
            GameObject.Destroy(gameObject);
        }

        public virtual void OnRollbackDestroy(){
            GameObject.Destroy(gameObject);
        }
    }
}