using System;
using Lockstep.Collision2D;
using Lockstep.Math;

namespace LockstepTutorial {
    
    [Serializable]
    public class CBrain : Component {
        public Entity target { get; private set; }
        public LFloat stopDistSqr = 1 * 1;
        public LFloat atkInterval = 1;
        private LFloat atkTimer;

        public override void DoUpdate(LFloat deltaTime){     
            if (!entity.rigidbody.isOnFloor) {
                return;
            }
            //find target
            var allPlayer = GameManager.allPlayers;
            var minDist = LFloat.MaxValue;
            Entity minTarget = null;
            foreach (var player in allPlayer) {
                if (player.isDead) continue;
                var dist = (player.transform.pos - transform.pos).sqrMagnitude;
                if (dist < minDist) {
                    minTarget = player;
                    minDist = dist;
                }
            }

            target = minTarget;
            if (minTarget == null)
                return;
            if (minDist > stopDistSqr) {
                // turn to target
                var targetPos = minTarget.transform.pos;
                var currentPos = transform.pos;
                var turnVal = entity.turnSpd * deltaTime;
                var targetDeg = CTransform2D.TurnToward(targetPos, currentPos, transform.deg, turnVal,
                    out var isFinishedTurn);
                transform.deg = targetDeg;
                //move to target
                var distToTarget = (targetPos - currentPos).magnitude;
                var movingStep = entity.moveSpd * deltaTime;
                if (movingStep > distToTarget) {
                    movingStep = distToTarget;
                }

                var toTarget = (targetPos - currentPos).normalized;
                transform.pos = transform.pos + toTarget * movingStep;
            }
            else {
                //atk target
                atkTimer -= deltaTime;
                if (atkTimer <= 0) {
                    atkTimer = atkInterval;
                    //Atk
                    target.TakeDamage(entity.damage, target.transform.Pos3);
                }
            }
        }
    }
}