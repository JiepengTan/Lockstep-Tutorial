using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.Game {
    public partial class CSkillBox : IBackup {
        public void WriteBackup(Serializer writer){
            writer.Write(skillConfigId);
            writer.Write(isFiring);
            writer.Write(_curSkillIdx);
            writer.Write(_skills.Count);
            for (int i = 0; i < _skills.Count; i++) {
                _skills[i].WriteBackup(writer);
            }
        }

        public void ReadBackup(Deserializer reader){
            skillConfigId = reader.ReadInt32();
            isFiring = reader.ReadBoolean();
            _curSkillIdx = reader.ReadInt32();
            _skills = new List<Skill>(reader.ReadInt32());
            for (int i = 0; i < _skills.Count; i++) {
                var skill = new Skill();
                skill.ReadBackup(reader);
                _skills[i] = skill;
            }
        }
    }

    [Serializable]
    [SelfImplement]
    public partial class CSkillBox : Component, ISkillEventHandler {
        public int skillConfigId;
        public bool isFiring;
        [Backup] private int _curSkillIdx = 0;
        [ReRefBackup] public SkillBoxConfig config;
        [Backup] private List<Skill> _skills = new List<Skill>();
        public Skill curSkill => (_curSkillIdx >= 0) ? _skills[_curSkillIdx] : null;

#if UNITY_EDITOR
        [UnityEngine.SerializeField]
#endif
        public override void DoStart(){
            base.DoStart();
            if (config != null) {
                config.CheckInit();
                foreach (var info in config.skillInfos) {
                    var skill = new Skill();
                    skill.DoStart(entity, info, this);
                    _skills.Add(skill);
                }
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            foreach (var skill in _skills) {
                skill.DoUpdate(deltaTime);
            }
        }

        public bool Fire(int idx){
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
#if UNITY_EDITOR
            foreach (var skill in _skills) {
                skill.OnDrawGizmos();
            }
#endif
        }
    }
}