using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    [Serializable]
    public class SkillColliderInfo {
        public LVector2 pos;
        public LVector2 size;
        public LFloat radius;
        public LFloat deg = new LFloat(180);
        public LFloat maxY;

        public bool IsCircle => radius > 0;
    }

    [Serializable]
    public class SkillPart {
        public bool _DebugShow;
        public LFloat moveSpd;
        public LFloat startFrame;
        public LFloat startTimer => startFrame * SkillPart.AnimFrameScale;
        public SkillColliderInfo collider;
        public LVector3 impulseForce;
        public bool needForce;
        public bool isResetForce;

        public LFloat interval;
        public int otherCount;
        public int damage;
        public static LFloat AnimFrameScale = new LFloat(true, 1667);
        [HideInInspector] public LFloat DeadTimer => startTimer + interval * (otherCount + LFloat.half);

        public LFloat NextTriggerTimer(int counter){
            return startTimer + interval * counter;
        }
    }

    [Serializable]
    public class SkillInfo {
        public string animName;
        public LFloat CD;
        public LFloat doneDelay;
        public int targetLayer;
        public LFloat maxPartTime;
        public List<SkillPart> parts = new List<SkillPart>();

        public void DoInit(){
            parts.Sort((a, b) => LMath.Sign(a.startFrame - b.startFrame));
            var time = LFloat.MinValue;
            foreach (var part in parts) {
                var partDeadTime = part.DeadTimer;
                if (partDeadTime > time) {
                    time = partDeadTime;
                }
            }

            maxPartTime = time + doneDelay;
        }
    }
}