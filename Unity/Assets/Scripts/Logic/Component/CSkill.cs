#define DEBUG_SKILL
using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using LockstepTutorial;
#if UNITY_EDITOR
using UnityEngine;
#endif
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic {

    public class CSkillContainer {
        public List<CSkill> skills = new List<CSkill>();
        public bool isFiring;
        public CSkill curSkill;

        public void Fire(int idx){
            if (idx < 0 || idx > skills.Count) {
                return;
            }

            var skill = skills[idx];
            if (skill.Fire()) {
                curSkill = skill;
            }
        }

        public void Stop(int idx){
            if (idx < 0 || idx > skills.Count) {
                return;
            }

            if (curSkill != null) {
                if (curSkill == skills[idx]) {
                    curSkill.Stop();
                }
            }
        }
        public void OnDrawGizmos(){
#if UNITY_EDITOR
            foreach (var skill in skills) {
                skill.OnDrawGizmos();
            }
#endif
        }
    }

    public class CSkill {
        public enum ESkillState {
            Idle,
            Firing,
        }

        private static readonly HashSet<ColliderProxy> _tempTargets = new HashSet<ColliderProxy>();

        public BaseActor owner; //=> view?.owner;
        public SkillInfo SkillInfo;
        public LFloat CD => SkillInfo.CD;
        public LFloat DoneDelay => SkillInfo.doneDelay;
        public List<SkillPart> Parts => SkillInfo.parts;
        public int TargetLayer => SkillInfo.targetLayer;
        public LFloat MaxPartTime => SkillInfo.maxPartTime;
        public string AnimName => SkillInfo.animName;
        public LFloat CdTimer { get; private set; }

        private ESkillState _state;
        private LFloat _skillTimer;
        private SkillPart _curPart;

#if DEBUG_SKILL
        private float _showTimer;
#endif

        public void Stop(){ }

        //  private PlayerView view;

        void Start(){
            // view = GetComponent<PlayerView>();
            _skillTimer = MaxPartTime;
        }


        public bool Fire(){
            if (CdTimer <= 0 && _state == ESkillState.Idle) {
                CdTimer = CD;
                _skillTimer = LFloat.zero;
                foreach (var part in Parts) {
                    part.counter = 0;
                }

                _state = ESkillState.Firing;
                owner.animator?.Play(AnimName);
                ((Player) owner).CMover.needMove = false;
                OnFire();
                return true;
            }

            return false;
        }

        public void OnFire(){
            owner.isInvincible = true;
            owner.isFire = true;
        }

        public void Done(){
            owner.isFire = false;
            owner.isInvincible = false;
            _state = ESkillState.Idle;
            owner.animator?.Play(AnimDefine.Idle);
        }

        public void DoUpdate(LFloat deltaTime){
            CdTimer -= deltaTime;
            _skillTimer += deltaTime;
            if (_skillTimer < MaxPartTime) {
                foreach (var part in Parts) {
                    CheckSkillPart(part);
                }
            }
            else {
                _curPart = null;
                if (_state == ESkillState.Firing) {
                    Done();
                }
            }

#if DEBUG_SKILL
            if (_showTimer < Time.realtimeSinceStartup) {
                _curPart = null;
            }
#endif
        }

        void CheckSkillPart(SkillPart part){
            if (part.counter > part.otherCount) return;
            if (_skillTimer > part.NextTriggerTimer()) {
                TriggerPart(part);
                part.counter++;
            }
        }

        void TriggerPart(SkillPart part){
            _curPart = part;
#if DEBUG_SKILL
            _showTimer = Time.realtimeSinceStartup + 0.1f;
#endif

            var col = part.collider;
            if (col.radius > 0) {
                //circle
                CollisionManager.QueryRegion(TargetLayer, owner.transform.TransformPoint(col.pos), col.radius,
                    _OnTriggerEnter);
            }
            else {
                //aabb
                CollisionManager.QueryRegion(TargetLayer, owner.transform.TransformPoint(col.pos), col.size,
                    owner.transform.forward,
                    _OnTriggerEnter);
            }

            foreach (var other in _tempTargets) {
                other.Entity.TakeDamage(_curPart.damage, other.Entity.transform.pos.ToLVector3());
            }

            //add force
            if (part.needForce) {
                var force = part.impulseForce;
                var forward = owner.transform.forward;
                var right = forward.RightVec();
                var z = forward * force.z + right * force.x;
                force.x = z.x;
                force.z = z.y;
                foreach (var other in _tempTargets) {
                    other.Entity.rigidbody.AddImpulse(force);
                }
            }

            if (part.isResetForce) {
                foreach (var other in _tempTargets) {
                    other.Entity.rigidbody.ResetSpeed(new LFloat(3));
                }
            }

            _tempTargets.Clear();
        }


        private void _OnTriggerEnter(ColliderProxy other){
            if (_curPart.collider.IsCircle && _curPart.collider.deg > 0) {
                var deg = (other.Transform2D.pos - owner.transform.pos).ToDeg();
                var degDiff = owner.transform.deg.Abs() - deg;
                if (LMath.Abs(degDiff) <= _curPart.collider.deg) {
                    _tempTargets.Add(other);
                }
            }
            else {
                _tempTargets.Add(other);
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos(){
#if DEBUG_SKILL
            float tintVal = 0.3f;
            Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);
            if (Application.isPlaying) {
                if (owner == null) return;
                if (_curPart == null) return;
                ShowPartGizmons(_curPart);
            }
            else {
                foreach (var part in Parts) {
                    if (part._DebugShow) {
                        ShowPartGizmons(part);
                    }
                }
            }

            Gizmos.color = Color.white;
#endif
        }

        private void ShowPartGizmons(SkillPart part){
            var col = part.collider;
            if (col.radius > 0) {
                //circle
                var pos = owner?.transform.TransformPoint(col.pos) ?? col.pos;
                Gizmos.DrawSphere(pos.ToVector3XZ(LFloat.one), col.radius.ToFloat());
            }
            else {
                //aabb
                var pos = owner?.transform.TransformPoint(col.pos) ?? col.pos;
                Gizmos.DrawCube(pos.ToVector3XZ(LFloat.one), col.size.ToVector3XZ(LFloat.one));
                DebugExtension.DebugLocalCube(Matrix4x4.TRS(
                        pos.ToVector3XZ(LFloat.one),
                        Quaternion.Euler(0, owner.transform.deg.ToFloat(), 0),
                        Vector3.one),
                    col.size.ToVector3XZ(LFloat.one), Gizmos.color);
            }
        }
#endif
    }
}