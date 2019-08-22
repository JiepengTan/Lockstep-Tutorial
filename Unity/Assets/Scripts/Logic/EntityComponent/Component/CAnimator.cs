using System;
using System.Collections.Generic;
using Lockstep.Logic;
using Lockstep.Collision2D;
using Lockstep.Math;
using LockstepTutorial;
using Debug = Lockstep.Logging.Debug;

public interface IAnimatorView {
    void Play(string name, bool isCross);
    void Sample(LFloat time);
}

[Serializable]
public class CAnimator : BaseComponent {
    public AnimatorConfig config;
    public IAnimatorView view;
    public AnimInfo curAnimInfo;
    public AnimBindInfo curAnimBindInfo;
    
    private LFloat _animLen;
    private LFloat _timer;
    private string _curAnimName;
    private int _curAnimIdx = -1;

    private List<string> _animNames = new List<string>();
    private LVector3 _intiPos;

    private List<AnimInfo> _animInfos => config.anims;
    public override void DoStart(){
        if(config == null) return;
        _animNames.Clear();
        foreach (var info in _animInfos) {
            _animNames.Add(info.name);
        }

        Play(AnimDefine.Idle);
    }

    public override void DoUpdate(LFloat deltaTime){
        if(config == null) return;
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
        if(config == null) return;
        if (_curAnimName == name)
            return;
        var idx = _animNames.IndexOf(name);
        if (idx == -1) {
            UnityEngine.Debug.LogError("miss animation " + name);
            return;
        }

        Debug.Trace($"{baseEntity.EntityId} PlayAnim {name} rawName {_curAnimName}");
        var hasChangedAnim = _curAnimName != name;
        _curAnimName = name;
        curAnimInfo = _animInfos[idx];
        curAnimBindInfo = config.events.Find((a) => a.name == name);
        if (curAnimBindInfo == null) curAnimBindInfo = AnimBindInfo.Empty;
        if (hasChangedAnim) {
            //owner.TakeDamage(0, owner.transform2D.Pos3);
            ResetAnim();
        }

        view?.Play(_curAnimName, isCrossfade);
    }

    public void SetTime(LFloat timer){
        if(config == null) return;
        var idx = GetTimeIdx(timer);
        _intiPos = transform.Pos3 - curAnimInfo[idx].pos;
        Debug.Trace(
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
        idx = Math.Min(curAnimInfo.OffsetCount - 1, idx);
        return idx;
    }
}