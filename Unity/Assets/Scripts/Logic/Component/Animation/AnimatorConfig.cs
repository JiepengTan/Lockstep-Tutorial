using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

[Serializable]
public class EventPointInfo {
    public LFloat timeStamp;
    public int eventId;
}

[Serializable]
public class AnimBindInfo {
    public static AnimBindInfo Empty = new AnimBindInfo(); 
    public string name;
    public bool isOnce; //default is loop
    public bool isMoveByAnim; //default is not
    public List<EventPointInfo> eventPoints = new List<EventPointInfo>();
}
[Serializable]
public struct AnimOffsetInfo {
    public LVector3 pos;
    public LFloat deg;

    public AnimOffsetInfo(LVector3 pos, LFloat deg){
        this.pos = pos;
        this.deg = deg;
    }

    public static AnimOffsetInfo operator -(AnimOffsetInfo a, AnimOffsetInfo b){
        return new AnimOffsetInfo(a.pos - b.pos, a.deg - b.deg);
    }

    public static AnimOffsetInfo operator +(AnimOffsetInfo a, AnimOffsetInfo b){
        return new AnimOffsetInfo(a.pos + b.pos, a.deg + b.deg);
    }
}
[Serializable]
public class AnimInfo {
    public string name;
    public LFloat length;
    public List<AnimOffsetInfo> offsets = new List<AnimOffsetInfo>();
    public int OffsetCount => offsets.Count;

    public void Add(AnimOffsetInfo info){
        offsets.Add(info);
    }

    public AnimOffsetInfo this[int index] {
        get => offsets[index];
        set => offsets[index] = value;
    }
}

[CreateAssetMenu(menuName = "AnimatorConfig")]
public class AnimatorConfig : ScriptableObject {
    public static readonly LFloat FrameInterval = new LFloat(true, 33);
    public List<AnimInfo> anims = new List<AnimInfo>();
    public List<AnimBindInfo> events = new List<AnimBindInfo>();
}