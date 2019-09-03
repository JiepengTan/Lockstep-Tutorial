using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Collision2D {
    public class TestRigidbody : MonoBehaviour {
        public CRigidbody CRigidbody;
        public CTransform2D CTransform2D;

        public LFloat G;
        public LVector3 force;
        
        public LFloat MinSleepSpeed = new LFloat(true, 100);
        public LFloat FloorFriction = new LFloat(3);
        public LFloat Mass = LFloat.one;
        public LFloat resetYSpd = new LFloat(true,100);
        private void Start(){
            CRigidbody = new CRigidbody();
            CTransform2D = new CTransform2D();
            CTransform2D.Pos3 = transform.position.ToLVector3();
            CRigidbody.BindRef(CTransform2D); 
            CRigidbody.DoStart();
        }

        private void Update(){
            CRigidbody.G = G;
            CRigidbody.MinSleepSpeed = MinSleepSpeed;
            CRigidbody.FloorFriction = FloorFriction;
            
            CRigidbody.Mass = Mass;
            CRigidbody.DoUpdate(Time.deltaTime.ToLFloat());
            transform.position = CTransform2D.Pos3.ToVector3();
        }

        public void AddImpulse(){
            CRigidbody.AddImpulse(force);
        }  
        public void ResetSpeed(LFloat ySpeed){
            CRigidbody.ResetSpeed(ySpeed);
        }
    }
}