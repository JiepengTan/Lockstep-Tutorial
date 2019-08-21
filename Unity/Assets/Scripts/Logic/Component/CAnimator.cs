using System;
using System.Collections.Generic;
using Lockstep.Logic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

public interface IAnimatorView {
    void Play(string name, bool isCross);
    void Sample(LFloat time);
}

public class CAnimator : BaseComponent {
    public IAnimatorView view;
    public AnimatorConfig config;

    private List<AnimInfo> _animInfos => config.anims;
    private LFloat _animLen;
    private LFloat _timer;
    public AnimInfo curAnimInfo;
    public AnimBindInfo curAnimBindInfo;
    private string curAnimName;
    private int curAnimIdx = -1;

    private List<string> animNames = new List<string>();
    private LVector3 intiPos;

    public override void DoStart(){
        animNames.Clear();
        foreach (var info in _animInfos) {
            animNames.Add(info.name);
        }

        Play(AnimDefine.Idle);
    }

    public override void DoUpdate(LFloat deltaTime){
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
            transform.Pos3 = (intiPos + pos.ToLVector3XZ(animOffset.y));
        }
    }

    public void SetTrigger(string name, bool isCrossfade = false){
        Play(name, isCrossfade); //TODO
    }

    public void Play(string name, bool isCrossfade = false){
        if (curAnimName == name)
            return;
        var idx = animNames.IndexOf(name);
        if (idx == -1) {
            UnityEngine.Debug.LogError("miss animation " + name);
            return;
        }

        Debug.Trace($"{entity.EntityId} PlayAnim {name} rawName {curAnimName}");
        var hasChangedAnim = curAnimName != name;
        curAnimName = name;
        curAnimInfo = _animInfos[idx];
        curAnimBindInfo = config.events.Find((a) => a.name == name);
        if (curAnimBindInfo == null) curAnimBindInfo = AnimBindInfo.Empty;
        if (hasChangedAnim) {
            //owner.TakeDamage(0, owner.transform2D.Pos3);
            ResetAnim();
        }

        view?.Play(curAnimName, isCrossfade);
    }

    private void ResetAnim(){
        _timer = LFloat.zero;
        SetTime(LFloat.zero);
    }

    public void SetTime(LFloat timer){
        var idx = GetTimeIdx(timer);
        intiPos = transform.Pos3 - curAnimInfo[idx].pos;
        Debug.Trace(
            $"{entity.EntityId} SetTime  idx:{idx} intiPos {entity.transform.Pos3}",
            true);
        this._timer = timer;
    }

    private int GetTimeIdx(LFloat timer){
        var idx = (int) (timer / AnimatorConfig.FrameInterval);
        idx = Math.Min(curAnimInfo.OffsetCount - 1, idx);
        return idx;
    }
}