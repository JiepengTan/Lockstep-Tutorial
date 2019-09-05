

using Lockstep.Math;

namespace Lockstep.Game {

    public partial class CommonStateService : ICommonStateService {
       public int Tick { get; set; }
       public LFloat DeltaTime { get;set;  }
       public LFloat TimeSinceGameStart { get; set;}
       public int Hash { get; set; }
       public bool IsPause { get; set; }

       public void SetTick(int val){ Tick = val;}
       public void SetDeltaTime(LFloat val) {DeltaTime = val; }
       public void SetTimeSinceGameStart(LFloat val){TimeSinceGameStart = val; }
    }

    public partial class ConstStateService : BaseService, IConstStateService {
        public static ConstStateService Instance { get; private set; }

        public ConstStateService(){
            Instance = this;
        }

        public bool IsVideoLoading { get; set; }
        public bool IsVideoMode { get; set; }
        public bool IsRunVideo { get; set; }
        public bool IsClientMode { get; set; }
        public bool IsReconnecting { get; set; }

        public bool IsPursueFrame { get; set; }

        public string GameName { get; set; }
        public int CurLevel { get; set; }
        public IContexts Contexts { get; set; }
        public int SnapshotFrameInterval { get; set; }
        public EPureModeType RunMode{ get; set; }
        public byte LocalActorId { get; set; }
        
        private string _clientConfigPath;
        public string ClientConfigPath => _clientConfigPath ?? (_clientConfigPath = _relPath + $"Data/Client/{GameName}/");

        private string _relPath = "";
        public string RelPath {
            get => _relPath;
            set => _relPath = value;
        }

    }
}