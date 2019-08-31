using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lockstep.Collision2D;
using Lockstep.Game;
using Lockstep.PathFinding;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
using Profiler = Lockstep.Util.Profiler;

namespace LockstepTutorial {
    public class MainManager : BaseService {
        public Simulator Simulator = new Simulator();
        public override void DoInit(object objParent){ }

        public override void InitReference(IServiceContainer serviceContainer, IManagerContainer mgrContainer){
            base.InitReference(serviceContainer, mgrContainer);
            Simulator.mgrContainer = mgrContainer;
        }

        public override void DoAwake(IServiceContainer services){
            Simulator.DoAwake(services);
        }

        public override void DoStart(){
            Simulator.DoStart();
        }

        public override void DoDestroy(){
            Simulator.DoDestroy();
        }

        public override void OnApplicationQuit(){
            Simulator.OnApplicationQuit();
        }

        public void DoUpdate(float deltaTime){
            Simulator.DoUpdate(deltaTime);
        }
    }

    public interface ISimulatorService : IService { }

    public class Simulator : BaseSystem, ISimulatorService {
        public string GameName;
        public static Simulator Instance { get; private set; }
        public static PlayerInput CurGameInput = new PlayerInput();

        public bool IsClientMode => _constStateService.IsClientMode;
        public bool IsReplay => _constStateService.IsReplay;
        public string recordFilePath;

        public PlayerServerInfo ClientModeInfo = new PlayerServerInfo();


        public IManagerContainer mgrContainer;
        private static int _maxServerFrameIdx;
        public int mapId;
        private bool _hasStart = false;
        [HideInInspector] public int predictTickCount = 3;
        [HideInInspector] public int inputTick;
        [HideInInspector] public int localPlayerId = 0;
        [HideInInspector] public int playerCount = 1;
        [HideInInspector] public int curMapId = 0;
        public int curFrameIdx = 0;
        [HideInInspector] public FrameInput curFrameInput;
        [HideInInspector] public PlayerServerInfo[] playerServerInfos;
        [HideInInspector] public List<FrameInput> frames = new List<FrameInput>();

        [Header("Ping")] public static int PingVal;
        public static List<float> Delays = new List<float>();
        public Dictionary<int, float> tick2SendTimer = new Dictionary<int, float>();

        public List<Enemy> AllEnemies => _gameStateService.GetEnemies();
        public List<Player> AllPlayers => _gameStateService.GetPlayers();

        public static Player MyPlayer;

        public static Transform MyPlayerTrans => MyPlayer?.engineTransform as Transform;
        [HideInInspector] public float remainTime; // remain time to update
        private NetClient netClient;
        private List<BaseSystem> _systems = new List<BaseSystem>();

        private static string _traceLogPath {
            get {
#if UNITY_STANDALONE_OSX
                return $"/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#else
                return $"c:/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#endif
            }
        }

        public void RegisterSystem(BaseSystem mgr){
            _systems.Add(mgr);
        }

        public void RegisterSystems(){
            RegisterSystem(new HeroSystem());
            RegisterSystem(new EnemySystem());
            RegisterSystem(new PhysicSystem());
            if (!IsReplay) {
                RegisterSystem(new TraceLogSystem());
            }
        }


        public override void DoAwake(IServiceContainer serviceContainer){
#if !UNITY_EDITOR
            IsReplay = false;
#endif
            _constStateService = serviceContainer.GetService<IConstStateService>();
            Instance = this;
            RegisterSystems();
            InitReference(serviceContainer, mgrContainer);
            foreach (var mgr in _systems) {
                mgr.InitReference(serviceContainer, mgrContainer);
            }

            _DoAwake(serviceContainer);
            foreach (var mgr in _systems) {
                mgr.DoAwake(serviceContainer);
            }
        }


        public override void DoStart(){
            _DoStart();
            foreach (var mgr in _systems) {
                mgr.DoStart();
            }

            Debug.Trace("Before StartGame _IdCounter" + BaseEntity.IdCounter);
            if (!IsReplay && !IsClientMode) {
                netClient = new NetClient();
                netClient.Start();
                netClient.Send(new Msg_JoinRoom() {name = Application.dataPath});
            }
            else {
                StartGame(0, playerServerInfos, localPlayerId);
            }
        }

        public void DoUpdate(float deltaTime){
            if (!_hasStart) return;
            remainTime += deltaTime;
            while (remainTime >= 0.03f) {
                remainTime -= 0.03f;
                //send input
                if (!IsReplay) {
                    SendInput();
                }


                if (GetFrame(curFrameIdx) == null) {
                    return;
                }

                Step();
            }
        }

        public override void DoDestroy(){
            netClient?.Send(new Msg_QuitRoom());
            foreach (var mgr in _systems) {
                mgr.DoDestroy();
            }

            if (!IsReplay) {
                RecordHelper.Serialize(recordFilePath, this);
            }

            Debug.FlushTrace();
            _DoDestroy();
        }


        public override void OnApplicationQuit(){ }


        public static void StartGame(Msg_StartGame msg){
            UnityEngine.Debug.Log("StartGame");
            Instance.StartGame(msg.mapId, msg.playerInfos, msg.localPlayerId);
        }

        public void StartGame(int mapId, PlayerServerInfo[] playerInfos, int localPlayerId){
            _hasStart = true;
            curMapId = mapId;

            this.playerCount = playerInfos.Length;
            this.playerServerInfos = playerInfos;
            this.localPlayerId = localPlayerId;
            Debug.TraceSavePath = _traceLogPath;
            AllPlayers.Clear();
            for (int i = 0; i < playerCount; i++) {
                Debug.Trace("CreatePlayer");
                AllPlayers.Add(new Player() {localId = i});
            }

            //create Players 
            for (int i = 0; i < playerCount; i++) {
                var playerInfo = playerInfos[i];
                _gameEntityService.CreatePlayer(AllPlayers[i], playerInfo.PrefabId, playerInfo.initPos);
            }

            MyPlayer = AllPlayers[localPlayerId];
        }


        public void SendInput(){
            if (IsClientMode) {
                PushFrameInput(new FrameInput() {
                    tick = curFrameIdx,
                    inputs = new PlayerInput[] {CurGameInput}
                });
                return;
            }

            predictTickCount = 2; //Mathf.Clamp(Mathf.CeilToInt(pingVal / 30), 1, 20);
            if (inputTick > predictTickCount + _maxServerFrameIdx) {
                return;
            }

            var playerInput = CurGameInput;
            netClient?.Send(new Msg_PlayerInput() {
                input = playerInput,
                tick = inputTick
            });
            //UnityEngine.Debug.Log("" + playerInput.inputUV);
            tick2SendTimer[inputTick] = Time.realtimeSinceStartup;
            //UnityEngine.Debug.Log("SendInput " + inputTick);
            inputTick++;
        }


        private void Step(){
            UpdateFrameInput();
            if (IsReplay) {
                if (curFrameIdx < frames.Count) {
                    Replay(curFrameIdx);
                    curFrameIdx++;
                }
            }
            else {
                Recorder();
                //send hash
                netClient?.Send(new Msg_HashCode() {
                    tick = curFrameIdx,
                    hash = GetHash()
                });
                curFrameIdx++;
            }
        }

        private void Recorder(){
            UpdateFrame();
        }


        private void Replay(int frameIdx){
            UpdateFrame();
        }

        private void UpdateFrame(){
            var deltaTime = new LFloat(true, 30);
            _DoUpdate(deltaTime);
            foreach (var system in _systems) {
                if (system.enable) {
                    system.DoUpdate(deltaTime);
                }
            }
        }

        public void _DoAwake(IServiceContainer services){ }


        public void _DoStart(){
            if (IsReplay) {
                RecordHelper.Deserialize(recordFilePath, this);
            }

            if (IsClientMode) {
                playerCount = 1;
                localPlayerId = 0;
                playerServerInfos = new PlayerServerInfo[] {ClientModeInfo};
                frames = new List<FrameInput>();
            }
        }


        public void _DoUpdate(LFloat deltaTime){ }

        public void _DoDestroy(){
            //DumpPathFindReqs();
        }


        public static void PushFrameInput(FrameInput input){
            var frames = Instance.frames;
            for (int i = frames.Count; i <= input.tick; i++) {
                frames.Add(new FrameInput());
            }

            if (frames.Count == 0) {
                Instance.remainTime = 0;
            }

            _maxServerFrameIdx = Math.Max(_maxServerFrameIdx, input.tick);
            if (Instance.tick2SendTimer.TryGetValue(input.tick, out var val)) {
                Delays.Add(Time.realtimeSinceStartup - val);
            }

            frames[input.tick] = input;
        }


        public FrameInput GetFrame(int tick){
            if (frames.Count > tick) {
                var frame = frames[tick];
                if (frame != null && frame.tick == tick) {
                    return frame;
                }
            }

            return null;
        }

        private void UpdateFrameInput(){
            curFrameInput = GetFrame(curFrameIdx);
            var frame = curFrameInput;
            for (int i = 0; i < playerCount; i++) {
                AllPlayers[i].input = frame.inputs[i];
            }
        }


        //{string.Format("{0:yyyyMMddHHmmss}", DateTime.Now)}_
        public int GetHash(){
            int hash = 1;
            int idx = 0;
            foreach (var entity in AllPlayers) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            foreach (var entity in AllEnemies) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            return hash;
        }
    }
}