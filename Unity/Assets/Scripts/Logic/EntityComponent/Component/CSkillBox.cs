using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Logic;
using Lockstep.Math;

namespace LockstepTutorial {
    [Serializable]
    public class CSkillBox : Component, ISkillEventHandler {
        public SkillBoxConfig config;
        public bool isFiring;
        public Skill curSkill;
        private int curSkillIdx = 0;

#if UNITY_EDITOR
        [UnityEngine.SerializeField]
#endif
        private List<Skill> skills;

        public override void DoStart(){
            base.DoStart();
            skills = new List<Skill>();
            if (config != null) {
                config.CheckInit();
                foreach (var info in config.skillInfos) {
                    var skill = new Skill();
                    skill.DoStart(entity, info, this);
                    skills.Add(skill);
                }
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            foreach (var skill in skills) {
                skill.DoUpdate(deltaTime);
            }
        }

        public bool Fire(int idx){
            if (idx < 0 || idx > skills.Count) {
                return false;
            }

            //Debug.Log("TryFire " + idx);

            if (isFiring) return false; //
            var skill = skills[idx];
            if (skill.Fire()) {
                curSkillIdx = idx;
                return true;
            }

            Debug.Log($"TryFire failure {idx} {skill.CdTimer}  {skill._state}");
            return false;
        }

        public void ForceStop(int idx = -1){
            if (idx == -1) {
                idx = curSkillIdx;
            }

            if (idx < 0 || idx > skills.Count) {
                return;
            }

            if (curSkill != null) {
                if (curSkill == skills[idx]) {
                    curSkill.ForceStop();
                }
            }
        }

        public void OnSkillStart(Skill skill){
            Debug.Log("OnSkillStart " + skill.SkillInfo.animName);
            curSkill = skill;
            isFiring = true;
            entity.isInvincible = true;
        }

        public void OnSkillDone(Skill skill){
            Debug.Log("OnSkillDone " + skill.SkillInfo.animName);
            curSkill = null;
            isFiring = false;
            entity.isInvincible = false;
        }

        public void OnSkillPartStart(Skill skill){
            //Debug.Log("OnSkillPartStart " + skill.SkillInfo.animName );
        }

        public void OnDrawGizmos(){
#if UNITY_EDITOR
            foreach (var skill in skills) {
                skill.OnDrawGizmos();
            }
#endif
        }
    }
}