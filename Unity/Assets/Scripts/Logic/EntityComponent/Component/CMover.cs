using System;
using Lockstep.Collision2D;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    
    [Serializable]
    public partial class CMover : Component {
        public Player player => (Player) entity;
        public PlayerInput input => player.input;

        
        static LFloat _sqrStopDist = new LFloat(true, 40);
        public LFloat speed => player.moveSpd;
        public bool hasReachTarget = false;
        public bool needMove = true;

        public override void DoUpdate(LFloat deltaTime){
            if (!entity.rigidbody.isOnFloor) {
                return;
            }

            var needChase = input.inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needChase) {
                var dir = input.inputUV.normalized;
                transform.pos = transform.pos + dir * speed * deltaTime;
                var targetDeg = dir.ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, player.turnSpd * deltaTime, out var hasReachDeg);
            }

            hasReachTarget = !needChase;
        }
    }
}