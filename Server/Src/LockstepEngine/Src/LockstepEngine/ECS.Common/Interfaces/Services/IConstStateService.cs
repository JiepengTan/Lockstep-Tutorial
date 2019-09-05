using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {

    public interface ICommonStateService : IService {
        int Tick { get; }
        LFloat DeltaTime { get;  }
        LFloat TimeSinceGameStart { get; }
        int Hash { get; set; }
        bool IsPause { get; set; }

        void SetTick(int val);
        void SetDeltaTime(LFloat val);
        void SetTimeSinceGameStart(LFloat val);
    }

    public interface IConstStateService : IService {
        bool IsVideoLoading { get; set; }
        bool IsVideoMode { get; set; }
        bool IsRunVideo { get; set; }
        bool IsClientMode { get; set; }
        
        /// 是否正在重连
        bool IsReconnecting { get; set; }

        /// 是否正在追帧
        bool IsPursueFrame { get; set; }
        string GameName { get; set; }
        
        int CurLevel { get; set; }
        IContexts Contexts { get; set; }
        int SnapshotFrameInterval { get; set; }
        EPureModeType RunMode { get; set; }
        string ClientConfigPath { get; }
        string RelPath { get; set; }

        byte LocalActorId { get; set; }
    }
}