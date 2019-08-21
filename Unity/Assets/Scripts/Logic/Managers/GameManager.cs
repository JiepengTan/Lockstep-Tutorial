using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.PathFinding;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
using Profiler = Lockstep.Util.Profiler;

namespace LockstepTutorial {
    public class GameManager : UnityBaseManager {
        public static GameManager Instance { get; private set; }
        public static PlayerInput CurGameInput = new PlayerInput();

        [Header("ClientMode")] public bool IsClientMode;
        public PlayerServerInfo ClientModeInfo = new PlayerServerInfo();

        [Header("Recorder")] public bool IsReplay = false;
        public string recordFilePath;

        private static int _maxServerFrameIdx;
        [Header("FrameData")] public int mapId;
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

        [Header("GameData")] public static List<Player> allPlayers = new List<Player>();
        public static Player MyPlayer;
        public static Transform MyPlayerTrans;
        [HideInInspector] public float remainTime; // remain time to update
        private NetClient netClient;
        private List<UnityBaseManager> _mgrs = new List<UnityBaseManager>();

        private static string _traceLogPath {
            get {
#if UNITY_STANDALONE_OSX
                return $"/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#else
                return $"c:/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#endif
            }
        }


        public void RegisterManagers(UnityBaseManager mgr){
            _mgrs.Add(mgr);
        }

        private void Awake(){
            Screen.SetResolution(1024, 768, false);
            gameObject.AddComponent<PingMono>();
            gameObject.AddComponent<InputMono>();
           
            Lockstep.Logging.Logger.OnMessage += UnityLogHandler.OnLog;
            _Awake();
        }

        private void Start(){
            _Start();
        }

        private void Update(){
            _DoUpdate();
        }

        private void _Awake(){
#if !UNITY_EDITOR
            IsReplay = false;
#endif
            DoAwake();
            foreach (var mgr in _mgrs) {
                mgr.DoAwake();
            }
        }


        private void _Start(){
            DoStart();
            foreach (var mgr in _mgrs) {
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


        private void _DoUpdate(){
            if (!_hasStart) return;
            remainTime += Time.deltaTime;
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
            allPlayers.Clear();
            for (int i = 0; i < playerCount; i++) {
                Debug.Trace("CreatePlayer");
                allPlayers.Add(new Player() {localId = i});
            }

            //create Players 
            for (int i = 0; i < playerCount; i++) {
                var playerInfo = playerInfos[i];
                var go = HeroManager.InstantiateEntity(allPlayers[i], playerInfo.PrefabId, playerInfo.initPos);
                //init mover
                if (allPlayers[i].localId == localPlayerId) {
                    MyPlayerTrans = go.transform;
                }
            }

            MyPlayer = allPlayers[localPlayerId];
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
                Recoder();
                //send hash
                netClient?.Send(new Msg_HashCode() {
                    tick = curFrameIdx,
                    hash = GetHash()
                });
                TraceHelper.TraceFrameState();
                curFrameIdx++;
            }
        }

        private void Recoder(){
            _Update();
        }


        private void Replay(int frameIdx){
            _Update();
        }

        private void _Update(){
            var deltaTime = new LFloat(true, 30);
            DoUpdate(deltaTime);
            foreach (var mgr in _mgrs) {
                mgr.DoUpdate(deltaTime);
            }
        }


        private void OnDestroy(){
            netClient?.Send(new Msg_QuitRoom());
            foreach (var mgr in _mgrs) {
                mgr.DoDestroy();
            }

            if (!IsReplay) {
                RecordHelper.Serialize(recordFilePath, this);
            }

            Debug.FlushTrace();
            DoDestroy();
        }

        public override void DoAwake(){
            Instance = this;
            var mgrs = GetComponents<UnityBaseManager>();
            foreach (var mgr in mgrs) {
                if (mgr != this) {
                    RegisterManagers(mgr);
                }
            }
        }


        public override void DoStart(){
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


        public override void DoUpdate(LFloat deltaTime){ }

        public override void DoDestroy(){
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
                allPlayers[i].InputAgent = frame.inputs[i];
            }
        }


        //{string.Format("{0:yyyyMMddHHmmss}", DateTime.Now)}_
        public int GetHash(){
            int hash = 1;
            int idx = 0;
            foreach (var entity in allPlayers) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            foreach (var entity in EnemyManager.Instance.allEnemy) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            return hash;
        }
    }
}