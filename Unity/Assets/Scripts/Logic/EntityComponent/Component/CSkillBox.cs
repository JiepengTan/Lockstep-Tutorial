using System;
using System.Collections.Generic;
using Lockstep.Logic;

namespace LockstepTutorial {
    [Serializable]
    public class CSkillBox : Component ,ISkillEventHandler {
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
                foreach (var info in config.skillInfos) {
                    var skill = new Skill();
                    skill.DoStart(entity, info,this);
                    skills.Add(skill);
                }
            }
        }

        public bool Fire(int idx){
            if (idx < 0 || idx > skills.Count) {
                return false;
            }

            if (isFiring) return false;//
            var skill = skills[idx];
            if (skill.Fire()) {
                curSkillIdx = idx;
                return true;
            }

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
            curSkill = skill;
            isFiring = true;
            entity.isInvincible = true;
        }

        public void OnSkillDone(Skill skill){
            curSkill = null;
            isFiring = false;
            entity.isInvincible = false;
        }

        public void OnSkillPartStart(Skill skill){
            
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