using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lockstep.Math;
using Lockstep.Game;
using NetMsg.Common;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
using Profiler = Lockstep.Util.Profiler;

namespace Lockstep.Game {
    public class World : BaseSystem {
        public static World Instance { get; private set; }
        public int Tick { get; set; }
        public PlayerInput[] PlayerInputs => _gameStateService.GetPlayers().Select(a => a.input).ToArray();
        public static Player MyPlayer;
        public static object MyPlayerTrans => MyPlayer?.engineTransform;
        private List<BaseSystem> _systems = new List<BaseSystem>();
        private bool _hasStart = false;
        public int HashCode;

        private ServiceContainer _svcContainer;

        public void RollbackTo(int tick, int missFrameTick, bool isNeedClear = true){
            if (tick < 0) {
                Debug.LogError("Target Tick invalid!" + tick);
                return;
            }

            Debug.Log($" curTick {Tick} RevertTo {tick} {missFrameTick} {isNeedClear}");
            _timeMachineService.RollbackTo(tick);
            _commonStateService.SetTick(tick);
            Tick = tick;
            var hash = _commonStateService.Hash;
            var curHash = GetHash();
            if (hash != curHash) {
                Debug.LogError($"Rollback error: Hash isDiff oldHash ={hash}  curHash{curHash}");
            }
        }

        public void StartSimulate(IServiceContainer serviceContainer, IManagerContainer mgrContainer){
            Instance = this;
            _serviceContainer = serviceContainer;
            _svcContainer = serviceContainer as ServiceContainer;
            RegisterSystems();
            if (!serviceContainer.GetService<IConstStateService>().IsVideoMode) {
                RegisterSystem(new TraceLogSystem());
            }

            InitReference(serviceContainer, mgrContainer);
            foreach (var mgr in _systems) {
                mgr.InitReference(serviceContainer, mgrContainer);
            }

            foreach (var mgr in _systems) {
                mgr.DoAwake(serviceContainer);
            }

            DoAwake(serviceContainer);
            foreach (var mgr in _systems) {
                mgr.DoStart();
            }

            DoStart();
        }

        public void StartGame(Msg_G2C_GameStartInfo gameStartInfo, int localPlayerId){
            if (_hasStart) return;
            _hasStart = true;
            var playerInfos = gameStartInfo.UserInfos;
            var playerCount = playerInfos.Length;
            string _traceLogPath = "";
#if UNITY_STANDALONE_OSX
            _traceLogPath = $"/tmp/LPDemo/Dump_{localPlayerId}.txt";
#else
            _traceLogPath = $"c:/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#endif
            Debug.TraceSavePath = _traceLogPath;

            Debug.Trace("CreatePlayer " + playerCount);
            //create Players 
            for (int i = 0; i < playerCount; i++) {
                var PrefabId = 0; //TODO
                var initPos = LVector2.zero; //TODO
                var player = _gameStateService.CreateEntity<Player>(PrefabId, initPos);
                player.localId = i;
            }

            var allPlayers = _gameStateService.GetPlayers();

            MyPlayer = allPlayers[localPlayerId];
        }


        public void Simulate(bool isNeedGenSnap = true){
            Step();
        }

        public void Predict(bool isNeedGenSnap = true){
            Step();
        }

        public void CleanUselessSnapshot(int checkedTick){ }


        public override void DoDestroy(){
            foreach (var mgr in _systems) {
                mgr.DoDestroy();
            }

            Debug.FlushTrace();
        }


        public override void OnApplicationQuit(){
            DoDestroy();
        }

        public int GetHash(){
            int idx = 0;
            return _GetHash(ref idx);
        }

        public StringBuilder DumpStr(){
            var sb = new StringBuilder();
            sb.AppendLine("Tick : " + Tick + "--------------------");
            _DumpStr(sb, "");
            return sb;
        }

        private int _GetHash(ref int idx){
            int hashIdx = 0;
            int hashCode = 0;
            foreach (var svc in _svcContainer.GetAllServices()) {
                if (svc is IHashCode hashSvc) {
                    hashCode += hashSvc.GetHash(ref hashIdx) * PrimerLUT.GetPrimer(hashIdx++);
                }
            }

            return hashCode;
        }

        private void _DumpStr(System.Text.StringBuilder sb, string prefix){
            foreach (var svc in _svcContainer.GetAllServices()) {
                if (svc is IDumpStr hashSvc) {
                    sb.AppendLine(svc.GetType() + " --------------------");
                    hashSvc.DumpStr(sb, "\t" + prefix);
                }
            }
        }

        Dictionary<int, int> _tick2Hash = new Dictionary<int, int>();
        Dictionary<int, StringBuilder> _tick2RawDumpString = new Dictionary<int, StringBuilder>();
        Dictionary<int, StringBuilder> _tick2ResumeDumpString = new Dictionary<int, StringBuilder>();
#if UNITY_EDITOR
        public string dumpPath => Path.Combine(Application.dataPath, _gameConfigService.DumpStrPath);
#endif
        private void Step(){
            if (_commonStateService.IsPause) return;
            _commonStateService.SetTick(Tick);

            var hash = GetHash();
            if (_constStateService.IsClientMode) {
                if (_tick2Hash.TryGetValue(Tick, out var val)) {
                    _tick2ResumeDumpString[Tick] = DumpStr();
                    if (hash != val) {
                        Debug.LogError($"Tick : CurHash {hash} is different from oldHash {val}");
#if UNITY_EDITOR
                        var path = dumpPath + "/cur.txt";
                        var dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir)) {
                            Directory.CreateDirectory(dir);
                        }

                        var minTick = _tick2ResumeDumpString.Keys.Min();
                        StringBuilder sbResume = new StringBuilder();
                        StringBuilder sbRaw = new StringBuilder();
                        for (int i = minTick; i <= Tick; i++) {
                            sbRaw.AppendLine(_tick2RawDumpString[i].ToString());
                            sbResume.AppendLine(_tick2ResumeDumpString[i].ToString());
                        }

                        File.WriteAllText(dumpPath + "/resume.txt", sbResume.ToString());
                        File.WriteAllText(dumpPath + "/raw.txt", sbRaw.ToString());
                        _commonStateService.IsPause = true;
                        UnityEngine.Debug.Break();
#endif
                    }
                }
                else {
                    _tick2RawDumpString[Tick] = DumpStr();
                }
            }

            _commonStateService.Hash = hash;
            _tick2Hash[Tick] = hash;
            _timeMachineService.Backup(Tick);
            var deltaTime = new LFloat(true, 30);
            foreach (var system in _systems) {
                if (system.enable) {
                    system.DoUpdate(deltaTime);
                }
            }

            Tick++;
        }


        public void RegisterSystems(){
            RegisterSystem(new HeroSystem());
            RegisterSystem(new EnemySystem());
            RegisterSystem(new PhysicSystem());
            RegisterSystem(new HashSystem());
        }

        public void RegisterSystem(BaseSystem mgr){
            _systems.Add(mgr);
        }
    }
}