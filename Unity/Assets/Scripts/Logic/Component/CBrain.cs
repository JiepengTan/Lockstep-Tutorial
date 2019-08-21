using Lockstep.Collision2D;
using Lockstep.Math;

namespace LockstepTutorial {
    public class CBrain : ActorComponent {
        public BaseActor target;
        public LFloat stopDistSqr = 1 * 1;
        public LFloat atkInterval = 1;
        private LFloat atkTimer;

        public override void DoUpdate(LFloat deltaTime){
            //find target
            var allPlayer = GameManager.allPlayers;
            var minDist = LFloat.MaxValue;
            BaseActor minTarget = null;
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
                var turnVal = baseActor.turnSpd * deltaTime;
                var targetDeg = CTransform2D.TurnToward(targetPos, currentPos, transform.deg, turnVal,
                    out var isFinishedTurn);
                transform.deg = targetDeg;
                //move to target
                var distToTarget = (targetPos - currentPos).magnitude;
                var movingStep = baseActor.moveSpd * deltaTime;
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
                    target.TakeDamage(baseActor.damage, target.transform.Pos3);
                }
            }
        }
    }
}