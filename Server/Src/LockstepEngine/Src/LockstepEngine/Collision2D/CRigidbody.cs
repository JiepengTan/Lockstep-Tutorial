using System;
using Lockstep.Collision2D;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game {
    public delegate void OnFloorResultCallback(bool isOnFloor);

    [Serializable]
    public partial class CRigidbody : IComponent {
        public CTransform2D transform { get; private set; }
        public static LFloat G = new LFloat(10);
        public static LFloat MinSleepSpeed = new LFloat(true, 100);
        public static LFloat FloorFriction = new LFloat(20);
        public static LFloat MinYSpd = new LFloat(-10);
        public static LFloat FloorY = LFloat.zero;

        [ReRefBackup] public OnFloorResultCallback OnFloorEvent;

        public LVector3 Speed;
        public LFloat Mass = LFloat.one;
        public bool isEnable = true;
        public bool isSleep = false;
        public bool isOnFloor;

        public void BindRef(CTransform2D transform2D){
            this.transform = transform2D;
        }

        //private int __id;
        //private static int __idCount;
        public void DoStart(){
            //__id = __idCount++;
            LFloat y = LFloat.zero;
            isOnFloor = TestOnFloor(transform.Pos3, ref y);
            Speed = LVector3.zero;
            isSleep = isOnFloor;
        }

        public void DoUpdate(LFloat deltaTime){
            if (!isEnable) return;
            if (!TestOnFloor(transform.Pos3)) {
                isSleep = false;
            }

            if (!isSleep) {
                if (!isOnFloor) {
                    Speed.y -= G * deltaTime;
                    Speed.y = LMath.Max(MinYSpd, Speed.y);
                }

                var pos = transform.Pos3;
                pos += Speed * deltaTime;
                LFloat y = pos.y;
                //Test floor
                isOnFloor = TestOnFloor(transform.Pos3, ref y);
                if (isOnFloor && Speed.y <= 0) {
                    Speed.y = LFloat.zero;
                }

                if (Speed.y <= 0) {
                    pos.y = y;
                }

                //Test walls
                if (TestOnWall(ref pos)) {
                    Speed.x = LFloat.zero;
                    Speed.z = LFloat.zero;
                }

                if (isOnFloor) {
                    var speedVal = Speed.magnitude - FloorFriction * deltaTime;
                    speedVal = LMath.Max(speedVal, LFloat.zero);
                    Speed = Speed.normalized * speedVal;
                    if (speedVal < MinSleepSpeed) {
                        isSleep = true;
                    }
                }

                transform.Pos3 = pos;
            }
        }


        public void AddImpulse(LVector3 force){
            isSleep = false;
            Speed += force / Mass;
            //Debug.Log(__id+ " AddImpulse " + force  +" after " + Speed);
        }

        public void ResetSpeed(LFloat ySpeed){
            Speed = LVector3.zero;
            Speed.y = ySpeed;
        }

        public void ResetSpeed(){
            Speed = LVector3.zero;
        }

        private bool TestOnFloor(LVector3 pos, ref LFloat y){
            var onFloor = pos.y <= 0; //TODO check with scene
            if (onFloor) {
                y = LFloat.zero;
            }

            return onFloor;
        }

        private bool TestOnFloor(LVector3 pos){
            var onFloor = pos.y <= 0; //TODO check with scene
            return onFloor;
        }

        private bool TestOnWall(ref LVector3 pos){
            return false; //TODO check with scene
        }
    }
}