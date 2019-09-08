#define DEBUG_FRAME_DELAY
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lockstep.ECS;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using Lockstep.Game;
using NetMsg.Common;
#if UNITY_EDITOR
using UnityEngine;
#endif
using Debug = Lockstep.Logging.Debug;
using Logger = Lockstep.Logging.Logger;

namespace Lockstep.Game {
    public class SimulatorService : BaseGameService, ISimulatorService {
        public static SimulatorService Instance { get; private set; }
        public int __debugRockbackToTick;

        public const long MinMissFrameReqTickDiff = 10;
        public const long MaxSimulationMsPerFrame = 20;
        public const int MaxPredictFrameCount = 30;

        public int PingVal => _cmdBuffer?.PingVal ?? 0;
        public int DelayVal => _cmdBuffer?.DelayVal ?? 0;

        // components
        public World World => _world;
        private World _world;
        private IFrameBuffer _cmdBuffer;
        private HashHelper _hashHelper;
        private DumpHelper _dumpHelper;

        // game status
        private Msg_G2C_GameStartInfo _gameStartInfo;
        public byte LocalActorId { get;private set; }
        private byte[] _allActors;
        private int _actorCount => _allActors.Length;
        private PlayerInput[] _playerInputs => _world.PlayerInputs;
        public bool IsRunning { get; set; }

        /// frame count that need predict(TODO should change according current network's delay)
        public int FramePredictCount = 0; //~~~

        /// game init timestamp
        public long _gameStartTimestampMs = -1;
        private int _tickSinceGameStart;
        public int TargetTick => _tickSinceGameStart + FramePredictCount;

        // input presend
        public int PreSendInputCount = 1; //~~~
        public int inputTick = 0;
        public int inputTargetTick => _tickSinceGameStart + PreSendInputCount;


        
        //video mode
        private Msg_RepMissFrame _videoFrames;
        private bool _isInitVideo = false;
        private int _tickOnLastJumpTo;
        private long _timestampOnLastJumpToMs;

        //refs 
        private IManagerContainer _mgrContainer;
        private IServiceContainer _serviceContainer;


        public int snapshotFrameInterval = 1;
        private bool _hasRecvInputMsg;

        public SimulatorService(){
            Instance = this;
        }

        public override void InitReference(IServiceContainer serviceContainer, IManagerContainer mgrContainer){
            base.InitReference(serviceContainer, mgrContainer);
            _serviceContainer = serviceContainer;
            _mgrContainer = mgrContainer;
        }

        public override void DoStart(){
            snapshotFrameInterval = 1;
            if (_constStateService.IsVideoMode) {
                snapshotFrameInterval = _constStateService.SnapshotFrameInterval;
            }

            _cmdBuffer = new FrameBuffer(this,_networkService, 2000, snapshotFrameInterval, MaxPredictFrameCount);
            _world = new World();
            _dumpHelper = new DumpHelper(_serviceContainer, _world);
            _hashHelper = new HashHelper(_serviceContainer, _world, _networkService, _cmdBuffer);
        }

        public override void DoDestroy(){
            IsRunning = false;
            _dumpHelper.DumpAll();
        }


        public void OnGameCreate(int targetFps, byte localActorId, byte actorCount, bool isNeedRender = true){
            FrameBuffer.__debugMainActorID = localActorId;
            var allActors = new byte[actorCount];
            for (byte i = 0; i < actorCount; i++) {
                allActors[i] = i;
            }

            Debug.Log($"GameCreate " + LocalActorId);

            //Init game status
            //_localActorId = localActorId;
            _allActors = allActors;
            _constStateService.LocalActorId = LocalActorId;
            _world.StartSimulate(_serviceContainer, _mgrContainer);
            EventHelper.Trigger(EEvent.LevelLoadProgress, 1f);
        }


        public void StartSimulate(){
            if (IsRunning) {
                Debug.LogError("Already started!");
                return;
            }

            IsRunning = true;
            if (_constStateService.IsClientMode) {
                _gameStartTimestampMs = LTime.realtimeSinceStartupMS;
            }

            _world.StartGame(_gameStartInfo, LocalActorId);
            Debug.Log($"Game Start");
            EventHelper.Trigger(EEvent.SimulationStart, null);
            
            while (inputTick < PreSendInputCount) {
                SendInputs(inputTick++);
            }
        }


        public void JumpTo(int tick){
            if (tick + 1 == _world.Tick || tick == _world.Tick) return;
            tick = LMath.Min(tick, _videoFrames.frames.Length - 1);
            var time = LTime.realtimeSinceStartupMS + 0.05f;
            if (!_isInitVideo) {
                _constStateService.IsVideoLoading = true;
                while (_world.Tick < _videoFrames.frames.Length) {
                    var sFrame = _videoFrames.frames[_world.Tick];
                    Simulate(sFrame, true);
                    if (LTime.realtimeSinceStartupMS > time) {
                        EventHelper.Trigger(EEvent.VideoLoadProgress, _world.Tick * 1.0f / _videoFrames.frames.Length);
                        return;
                    }
                }

                _constStateService.IsVideoLoading = false;
                EventHelper.Trigger(EEvent.VideoLoadDone);
                _isInitVideo = true;
            }

            if (_world.Tick > tick) {
                RollbackTo(tick, _videoFrames.frames.Length, false);
            }

            while (_world.Tick <= tick) {
                var sFrame = _videoFrames.frames[_world.Tick];
                Simulate(sFrame, false);
            }

            _viewService.RebindAllEntities();
            _timestampOnLastJumpToMs = LTime.realtimeSinceStartupMS;
            _tickOnLastJumpTo = tick;
        }


        public void RunVideo(){
            if (_tickOnLastJumpTo == _world.Tick) {
                _timestampOnLastJumpToMs = LTime.realtimeSinceStartupMS;
                _tickOnLastJumpTo = _world.Tick;
            }

            var frameDeltaTime = (LTime.timeSinceLevelLoad - _timestampOnLastJumpToMs) * 1000;
            var targetTick = System.Math.Ceiling(frameDeltaTime / NetworkDefine.UPDATE_DELTATIME) + _tickOnLastJumpTo;
            while (_world.Tick <= targetTick) {
                if (_world.Tick < _videoFrames.frames.Length) {
                    var sFrame = _videoFrames.frames[_world.Tick];
                    Simulate(sFrame, false);
                }
                else {
                    break;
                }
            }
        }

        public void DoUpdate(float deltaTime){
            if (!IsRunning) {
                return;
            }

            if (_hasRecvInputMsg) {
                if (_gameStartTimestampMs == -1) {
                    _gameStartTimestampMs = LTime.realtimeSinceStartupMS;
                }
            }

            if (_gameStartTimestampMs <= 0) {
                return;
            }

            _tickSinceGameStart =
                (int) ((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);
            if (_constStateService.IsVideoMode) {
                return;
            }

            if (__debugRockbackToTick > 0) {
                GetService<ICommonStateService>().IsPause = true;
                RollbackTo(__debugRockbackToTick, 0, false);
                __debugRockbackToTick = -1;
            }

            if (_commonStateService.IsPause) {
                return;
            }

            _cmdBuffer.DoUpdate(deltaTime);

            //client mode no network
            if (_constStateService.IsClientMode) {
                DoClientUpdate();
            }
            else {
                while (inputTick <= inputTargetTick) {
                    SendInputs(inputTick++);
                }

                DoNormalUpdate();
            }
        }


        private void DoClientUpdate(){
            while (_world.Tick < TargetTick) {
                FramePredictCount = 0;
                var input = new Msg_PlayerInput(_world.Tick, LocalActorId, _inputService.GetInputCmds());
                var frame = new ServerFrame() {
                    tick = _world.Tick,
                    _inputs = new Msg_PlayerInput[] {input}
                };
                _cmdBuffer.PushLocalFrame(frame);
                _cmdBuffer.PushServerFrames(new ServerFrame[] {frame});
                Simulate(_cmdBuffer.GetFrame(_world.Tick));
            }
        }


        private void DoNormalUpdate(){
            //make sure client is not move ahead too much than server
            var maxContinueServerTick = _cmdBuffer.MaxContinueServerTick;
            if ((_world.Tick - maxContinueServerTick) > MaxPredictFrameCount) {
                return;
            }

            var minTickToBackup = (maxContinueServerTick - (maxContinueServerTick % snapshotFrameInterval));

            // Pursue Server frames
            var deadline = LTime.realtimeSinceStartupMS + MaxSimulationMsPerFrame;
            while (_world.Tick < _cmdBuffer.CurTickInServer) {
                var tick = _world.Tick;
                var sFrame = _cmdBuffer.GetServerFrame(tick);
                if (sFrame == null) {
                    OnPursuingFrame();
                    return;
                }

                _cmdBuffer.PushLocalFrame(sFrame);
                Simulate(sFrame, tick == minTickToBackup);
                if (LTime.realtimeSinceStartupMS > deadline) {
                    OnPursuingFrame();
                    return;
                }
            }

            if (_constStateService.IsPursueFrame) {
                _constStateService.IsPursueFrame = false;
                EventHelper.Trigger(EEvent.PursueFrameDone);
            }


            // Roll back
            if (_cmdBuffer.IsNeedRollback) {
                RollbackTo(_cmdBuffer.NextTickToCheck, maxContinueServerTick);
                CleanUselessSnapshot(System.Math.Min(_cmdBuffer.NextTickToCheck - 1, _world.Tick));

                minTickToBackup = System.Math.Max(minTickToBackup, _world.Tick + 1);
                while (_world.Tick <= maxContinueServerTick) {
                    var sFrame = _cmdBuffer.GetServerFrame(_world.Tick);
                    Logging.Debug.Assert(sFrame != null && sFrame.tick == _world.Tick,
                        $" logic error: server Frame  must exist tick {_world.Tick}");
                    _cmdBuffer.PushLocalFrame(sFrame);
                    Simulate(sFrame, _world.Tick == minTickToBackup);
                }
            }


            //Run frames
            while (_world.Tick <= TargetTick) {
                var curTick = _world.Tick;
                ServerFrame frame = null;
                var sFrame = _cmdBuffer.GetServerFrame(curTick);
                if (sFrame != null) {
                    frame = sFrame;
                }
                else {
                    var cFrame = _cmdBuffer.GetLocalFrame(curTick);
                    FillInputWithLastFrame(cFrame);
                    frame = cFrame;
                }

                _cmdBuffer.PushLocalFrame(frame);
                Predict(frame, true);
            }

            _hashHelper.CheckAndSendHashCodes();
        }

        void SendInputs(int curTick){
            var input = new Msg_PlayerInput(curTick, LocalActorId, _inputService.GetInputCmds());
            var cFrame = new ServerFrame();
            var inputs = new Msg_PlayerInput[_actorCount];
            inputs[LocalActorId] = input;
            cFrame.Inputs = inputs;
            cFrame.tick = curTick;
            FillInputWithLastFrame(cFrame);
            _cmdBuffer.PushLocalFrame(cFrame);
            //if (input.Commands != null) {
            //    var playerInput = new Deserializer(input.Commands[0].content).Parse<Lockstep.Game.PlayerInput>();
            //    Debug.Log($"SendInput curTick{curTick} maxSvrTick{_cmdBuffer.MaxServerTickInBuffer} _tickSinceGameStart {_tickSinceGameStart} uv {playerInput.inputUV}");
            //}
            if (curTick > _cmdBuffer.MaxServerTickInBuffer) {
                //TODO combine all history inputs into one Msg 
                //Debug.Log("SendInput " + curTick +" _tickSinceGameStart " + _tickSinceGameStart);
                _cmdBuffer.SendInput(input);
            }
        }


        private void Simulate(ServerFrame frame, bool isNeedGenSnap = true){
            Step(frame, isNeedGenSnap);
        }

        private void Predict(ServerFrame frame, bool isNeedGenSnap = true){
            Step(frame, isNeedGenSnap);
        }

        private void RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true){
            _world.RollbackTo(tick, maxContinueServerTick, isNeedClear);
            var hash = _commonStateService.Hash;
            var curHash = _hashHelper.CalcHash();
            if (hash != curHash) {
                Debug.LogError($"Rollback error: Hash isDiff oldHash ={hash}  curHash{curHash}");
            }
        }


        void Step(ServerFrame frame, bool isNeedGenSnap = true){
            //Debug.Log("Step: " + _world.Tick + " TargetTick: " + TargetTick);
            _commonStateService.SetTick(_world.Tick);
            var hash = _hashHelper.CalcHash();
            _commonStateService.Hash = hash;
            _timeMachineService.Backup(_world.Tick);
            _hashHelper.SetHash(_world.Tick, hash);
            DumpFrame(hash);
            ProcessInputQueue(frame);
            _world.Step(isNeedGenSnap);
            var tick = _world.Tick;
            _cmdBuffer.SetClientTick(tick);
            //clean useless snapshot
            if (isNeedGenSnap && tick % snapshotFrameInterval == 0) {
                CleanUselessSnapshot(System.Math.Min(_cmdBuffer.NextTickToCheck - 1, _world.Tick));
            }
        }

        private void CleanUselessSnapshot(int tick){
            //TODO
        }

        private void DumpFrame(int hash){
            if (_constStateService.IsClientMode) {
                if (_hashHelper.TryGetValue(_world.Tick, out var val)) {
                    _dumpHelper.DumpFrame(false);
                    if (hash != val) {
                        Debug.LogError($"Tick : CurHash {hash} is different from oldHash {val}");
                        _dumpHelper.DumpToFile();
                        _commonStateService.IsPause = true;
                    }
                }
                else {
                    _dumpHelper.DumpFrame(true);
                }
            }
            else {
                _dumpHelper.DumpFrame(true);
            }
        }

        private void FillInputWithLastFrame(ServerFrame frame){
            int tick = frame.tick;
            var inputs = frame.Inputs;
            var lastServerInputs = tick == 0 ? null : _cmdBuffer.GetFrame(tick - 1)?.Inputs;
            var myInput = inputs[LocalActorId];
            //fill inputs with last frame's input (Input predict)
            for (int i = 0; i < _actorCount; i++) {
                inputs[i] = new Msg_PlayerInput(tick, _allActors[i], lastServerInputs?[i]?.Commands);
            }

            inputs[LocalActorId] = myInput;
        }

        private void ProcessInputQueue(ServerFrame frame){
            var inputs = frame.Inputs;
            foreach (var playerInput in _playerInputs) {
                playerInput.Reset();
            }

            foreach (var input in inputs) {
                if (input.Commands == null) continue;
                if (input.ActorId >= _playerInputs.Length) continue;
                var inputEntity = _playerInputs[input.ActorId];
                foreach (var command in input.Commands) {
                    Logger.Trace(this, input.ActorId + " >> " + input.Tick + ": " + input.Commands.Count());
                    _inputService.Execute(command, inputEntity);
                }
            }
        }

        void OnPursuingFrame(){
            _constStateService.IsPursueFrame = true;
            Debug.Log($"PurchaseServering curTick:" + _world.Tick);
            var progress = _world.Tick * 1.0f / _cmdBuffer.CurTickInServer;
            EventHelper.Trigger(EEvent.PursueFrameProcess, progress);
        }


        #region NetEvents

        void OnEvent_BorderVideoFrame(object param){
            _videoFrames = param as Msg_RepMissFrame;
        }

        void OnEvent_OnServerFrame(object param){
            var msg = param as Msg_ServerFrames;
            _hasRecvInputMsg = true;

            _cmdBuffer.PushServerFrames(msg.frames);
        }

        void OnEvent_OnServerMissFrame(object param){
            Debug.Log($"OnEvent_OnServerMissFrame");
            var msg = param as Msg_RepMissFrame;
            _cmdBuffer.PushMissServerFrames(msg.frames, false);
        }

        void OnEvent_OnPlayerPing(object param){
            var msg = param as Msg_G2C_PlayerPing;
            _cmdBuffer.OnPlayerPing(msg);
        }

        void OnEvent_OnServerHello(object param){
            var msg = param as Msg_G2C_Hello;
            LocalActorId = msg.LocalId;
            Debug.Log("OnEvent_OnServerHello " + LocalActorId);
        }

        void OnEvent_OnGameCreate(object param){
            if (param is Msg_G2C_Hello msg) {
                OnGameCreate(60, msg.LocalId, msg.UserCount);
            }

            if (param is Msg_G2C_GameStartInfo smsg) {
                _gameStartInfo = smsg;
                OnGameCreate(60, 0, smsg.UserCount);
            }

            EventHelper.Trigger(EEvent.SimulationInit, null);
        }


        void OnEvent_OnAllPlayerFinishedLoad(object param){
            Debug.Log($"OnEvent_OnAllPlayerFinishedLoad");
            StartSimulate();
        }

        void OnEvent_LevelLoadDone(object param){
            Debug.Log($"OnEvent_LevelLoadDone " + _constStateService.IsReconnecting);
            if (_constStateService.IsReconnecting || _constStateService.IsVideoMode ||
                _constStateService.IsClientMode) {
                StartSimulate();
            }
        }

        #endregion
    }
}