using System;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.Game;
using Lockstep.UnityExt;
using Debug = Lockstep.Logging.Debug;
#if UNITY_5_3_OR_NEWER
using HideInInspector = UnityEngine.HideInInspector;

#endif

namespace Lockstep.Game {
    public interface IAnimatorView {
        void Play(string name, bool isCross);
        void Sample(LFloat time);
    }


    [Serializable]
    public partial class CAnimator : Component {
        public int configId;

        [HideInInspector] [ReRefBackup] public AnimatorConfig config;
        [HideInInspector] [ReRefBackup] public IAnimatorView view;
        [HideInInspector] [ReRefBackup] public AnimBindInfo curAnimBindInfo;

        [Backup] private LFloat _animLen;
        [Backup] private LFloat _timer;
        [Backup] private string _curAnimName = "";
        [Backup] private int _curAnimIdx = -1;

        private List<string> _animNames = new List<string>();
        private LVector3 _intiPos;

        private List<AnimInfo> _animInfos => config.anims;
        public AnimInfo curAnimInfo => _curAnimIdx == -1 ? null : _animInfos[_curAnimIdx];

        public override void BindEntity(BaseEntity baseEntity){
            base.BindEntity(baseEntity);
            config = entity.GetService<IGameConfigService>().GetAnimatorConfig(configId);
            if (config == null) return;
            UpdateBindInfo();
            _animNames.Clear();
            foreach (var info in _animInfos) {
                _animNames.Add(info.name);
            }
        }

        void UpdateBindInfo(){
            curAnimBindInfo = config.events.Find((a) => a.name == _curAnimName);
            if (curAnimBindInfo == null) curAnimBindInfo = AnimBindInfo.Empty;
        }

        public override void DoStart(){
            Play(AnimDefine.Idle);
        }

        public override void DoUpdate(LFloat deltaTime){
            if (config == null) return;
            _animLen = curAnimInfo.length;
            _timer += deltaTime;
            if (_timer > _animLen) {
                ResetAnim();
            }

            view?.Sample(_timer);

            var idx = GetTimeIdx(_timer);
            if (curAnimBindInfo.isMoveByAnim) {
                var animOffset = curAnimInfo[idx].pos;
                var pos = transform.TransformDirection(animOffset.ToLVector2XZ());
                transform.Pos3 = (_intiPos + pos.ToLVector3XZ(animOffset.y));
            }
        }

        public void SetTrigger(string name, bool isCrossfade = false){
            Play(name, isCrossfade); //TODO
        }

        public void Play(string name, bool isCrossfade = false){
            if (config == null) return;
            if (_curAnimName == name)
                return;
            var idx = _animNames.IndexOf(name);
            if (idx == -1) {
                UnityEngine.Debug.LogError("miss animation " + name);
                return;
            }

            DebugService.Trace($"{baseEntity.EntityId} PlayAnim {name} rawName {_curAnimName}");
            var hasChangedAnim = _curAnimName != name;
            _curAnimName = name;
            _curAnimIdx = idx;
            UpdateBindInfo();
            if (hasChangedAnim) {
                //owner.TakeDamage(0, owner.transform2D.Pos3);
                ResetAnim();
            }

            view?.Play(_curAnimName, isCrossfade);
        }

        public void SetTime(LFloat timer){
            if (config == null) return;
            var idx = GetTimeIdx(timer);
            _intiPos = transform.Pos3 - curAnimInfo[idx].pos;
            DebugService.Trace(
                $"{baseEntity.EntityId} SetTime  idx:{idx} intiPos {baseEntity.transform.Pos3}",
                true);
            this._timer = timer;
        }

        private void ResetAnim(){
            _timer = LFloat.zero;
            SetTime(LFloat.zero);
        }


        private int GetTimeIdx(LFloat timer){
            var idx = (int) (timer / AnimatorConfig.FrameInterval);
            idx = System.Math.Min(curAnimInfo.OffsetCount - 1, idx);
            return idx;
        }
    }
}