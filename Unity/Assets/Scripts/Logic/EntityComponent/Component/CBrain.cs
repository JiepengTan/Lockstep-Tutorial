using System;
using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Game {
    [Serializable]
    public partial class CBrain : Component {
        public Entity target { get; private set; }
        public int targetId;
        public LFloat stopDistSqr = 1 * 1;
        public LFloat atkInterval = 1;
        [Backup] private LFloat _atkTimer;

        public override void BindEntity(BaseEntity e){
            base.BindEntity(e);
            target = GameStateService.GetEntity(targetId) as Entity;
        }

        public override void DoUpdate(LFloat deltaTime){
            if (!entity.rigidbody.isOnFloor) {
                return;
            }

            //find target
            var allPlayer = GameStateService.GetPlayers();
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
            targetId = target?.EntityId ?? -1;

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
                _atkTimer -= deltaTime;
                if (_atkTimer <= 0) {
                    _atkTimer = atkInterval;
                    //Atk
                    target.TakeDamage(entity, entity.damage, target.transform.Pos3);
                }
            }
        }
    }
}