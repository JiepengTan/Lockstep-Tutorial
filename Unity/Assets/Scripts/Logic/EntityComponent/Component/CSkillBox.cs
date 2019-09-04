using System;
using System.Collections.Generic;
using System.Text;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.UnityExt;
using Lockstep.Util;
using Debug = Lockstep.Logging.Debug;
#if UNITY_EDITOR
using HideInInspector = UnityEngine.HideInInspector;

#endif

namespace Lockstep.Game {

    [Serializable]
    public partial class CSkillBox : Component, ISkillEventHandler {
        public int configId;
        public bool isFiring;
        [HideInInspector] [ReRefBackup] public SkillBoxConfig config;
        [Backup] private int _curSkillIdx = 0;
        [Backup] private List<Skill> _skills = new List<Skill>();
        public Skill curSkill => (_curSkillIdx >= 0) ? _skills[_curSkillIdx] : null;

        public override void BindEntity(BaseEntity e){
            base.BindEntity(e);
            config = entity.GetService<IGameConfigService>().GetSkillConfig(configId);
            if (config == null) return;
            if (config.skillInfos.Count != _skills.Count) {
                //Debug.LogError("Skill count diff");
                _skills.Clear();
                foreach (var info in config.skillInfos) {
                    var skill = new Skill();
                    _skills.Add(skill);
                    skill.BindEntity(entity, info, this);
                    skill.DoStart();
                }
            }

            for (int i = 0; i < _skills.Count; i++) {
                var skill = _skills[i];
                skill.BindEntity(entity, config.skillInfos[i], this);
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            if (config == null) return;
            foreach (var skill in _skills) {
                skill.DoUpdate(deltaTime);
            }
        }

        public bool Fire(int idx){
            if (config == null) return false;
            if (idx < 0 || idx > _skills.Count) {
                return false;
            }

            //Debug.Log("TryFire " + idx);

            if (isFiring) return false; //
            var skill = _skills[idx];
            if (skill.Fire()) {
                _curSkillIdx = idx;
                return true;
            }

            Debug.Log($"TryFire failure {idx} {skill.CdTimer}  {skill.State}");
            return false;
        }

        public void ForceStop(int idx = -1){
            if (config == null) return;
            if (idx == -1) {
                idx = _curSkillIdx;
            }

            if (idx < 0 || idx > _skills.Count) {
                return;
            }

            if (curSkill != null) {
                curSkill.ForceStop();
            }
        }

        public void OnSkillStart(Skill skill){
            Debug.Log("OnSkillStart " + skill.SkillInfo.animName);
            isFiring = true;
            entity.isInvincible = true;
        }

        public void OnSkillDone(Skill skill){
            Debug.Log("OnSkillDone " + skill.SkillInfo.animName);
            isFiring = false;
            entity.isInvincible = false;
        }

        public void OnSkillPartStart(Skill skill){
            //Debug.Log("OnSkillPartStart " + skill.SkillInfo.animName );
        }

        public void OnDrawGizmos(){
            if (config == null) return;
#if UNITY_EDITOR
            foreach (var skill in _skills) {
                skill.OnDrawGizmos();
            }
#endif
        }
    }
}