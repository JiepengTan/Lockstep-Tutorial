using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep;
using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {
    [Serializable]
    public class BaseEntity : BaseLifeCycle, IEntity, ILPTriggerEventHandler {
        public static int IdCounter { get; private set; }
        public int EntityId { get; private set; }
        public int PrefabId;
        public object engineTransform;
        public CTransform2D transform { get; } = new CTransform2D();
        public CRigidbody rigidbody = new CRigidbody();
        protected List<BaseComponent> allComponents = new List<BaseComponent>();

        public BaseEntity(){
            Debug.Trace("BaseEntity  " + IdCounter.ToString(), true);
            rigidbody.transform = transform;
        }

        protected void RegisterComponent(BaseComponent comp){
            allComponents.Add(comp);
            comp.BindEntity(this);
        }

        public override void DoAwake(){
            EntityId = IdCounter++;
            foreach (var comp in allComponents) {
                comp.DoAwake();
            }
        }

        public override void DoStart(){
            rigidbody.DoStart();
            foreach (var comp in allComponents) {
                comp.DoStart();
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            rigidbody.DoUpdate(deltaTime);
            foreach (var comp in allComponents) {
                comp.DoUpdate(deltaTime);
            }
        }

        public override void DoDestroy(){
            foreach (var comp in allComponents) {
                comp.DoDestroy();
            }
        }

        public virtual void OnLPTriggerEnter(ColliderProxy other){ }

        public virtual void OnLPTriggerStay(ColliderProxy other){ }

        public virtual void OnLPTriggerExit(ColliderProxy other){ }
    }
}