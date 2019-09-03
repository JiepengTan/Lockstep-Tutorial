#define DEBUG_FRAME_DELAY
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lockstep.ECS;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using Lockstep.Game;
using NetMsg.Common;
using Logger = Lockstep.Logging.Logger;

namespace Lockstep.Game {
    public class SimulatorService : BaseGameService, ISimulatorService {
        public const int MinMissFrameReqTickDiff = 10;
        public const int MaxSimulationMsPerFrame = 20;

        public static int PingVal;
        public static List<float> Delays = new List<float>();

        public static SimulatorService Instance { get; private set; }

        public SimulatorService(){
            Instance = this;
        }

        public World World => _world;
        private World _world = new World();
        public bool IsRunning { get; set; }

        private byte _localActorId;

        private float _tickDt;
        private float _accumulatedTime;
        private FrameBuffer _cmdBuffer;
        private int _localTick;
        private int _gameId;
        private List<long> allHashCodes = new List<long>();
        private int _firstHashTick = 0;

        private byte[] _allActors;
        private int _actorCount;
        private int _curTick = 0;
        private Msg_RepMissFrame _videoFrames;
        private float _frameDeadline;

        private float _timestampOnPurcue;
        private int _tickOnPursue;
        private bool _isFinishPursue = false;
        private bool _isInitVideo = false;
        private float _timestampOnLastJumpTo;
        private int _tickOnLastJumpTo;

        private List<PlayerInput> _playerInputs => _world.PlayerInputs;
        private IManagerContainer _mgrContainer;
        private IServiceContainer _serviceContainer;
        private Msg_G2C_GameStartInfo _gameStartInfo;


        public override void InitReference(IServiceContainer serviceContainer, IManagerContainer mgrContainer){
            base.InitReference(serviceContainer, mgrContainer);
            _serviceContainer = serviceContainer;
            _mgrContainer = mgrContainer;
        }

        public override void DoStart(){
            _cmdBuffer = new FrameBuffer();
            if (_constStateService.IsVideoMode) {
                FrameBuffer.SnapshotFrameInterval = _constStateService.SnapshotFrameInterval;
            }
        }

        public override void DoDestroy(){
            IsRunning = false;
        }


        private void OnEvent_BorderVideoFrame(object param){
            _videoFrames = param as Msg_RepMissFrame;
        }

        void OnEvent_OnServerFrame(object param){
            var msg = param as Msg_ServerFrames;
            _cmdBuffer.PushServerFrames(msg.frames);
        }

        void OnEvent_OnServerMissFrame(object param){
            Debug.Log($"OnEvent_OnServerMissFrame");
            var msg = param as Msg_RepMissFrame;
            _cmdBuffer.PushServerFrames(msg.frames, false);
            _networkService.SendMissFrameRepAck(_cmdBuffer.GetMissServerFrameTick());
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

        public void StartSimulate(){
            if (IsRunning) {
                Debug.LogError("Already started!");
                return;
            }

            _world.StartGame(_gameStartInfo, _localActorId);
            IsRunning = true;
            SetPurchaseTimestamp();
            Debug.Log($"Game Start");
            EventHelper.Trigger(EEvent.SimulationStart, null);
        }


        private bool IsTimeout(){
            return LTime.realtimeSinceStartup > _frameDeadline;
        }


        public bool PursueServer(int minTickToBackup){
            if (_world.Tick >= _cmdBuffer.curServerTick)
                return true;
            while (_world.Tick <= _cmdBuffer.curServerTick) {
                var tick = _world.Tick;
                var sFrame = _cmdBuffer.GetServerFrame(tick);
                if (sFrame == null)
                    return false;
                _cmdBuffer.PushLocalFrame(sFrame);
                Simulate(sFrame, tick >= minTickToBackup);
                if (IsTimeout()) {
                    return false;
                }
            }

            SetPurchaseTimestamp();
            return true;
        }

        private void SetPurchaseTimestamp(){
            var ping = 35;
            var tickClientShouldPredict = 2; //(ping * 2) / NetworkDefine.UPDATE_DELTATIME + 1;
            _tickOnPursue = _world.Tick + tickClientShouldPredict;
            _timestampOnPurcue = LTime.realtimeSinceStartup;
        }

        public float GetPursueProgress(){
            return _world.Tick * 1.0f / _cmdBuffer.curServerTick;
        }

        private float remainTime = 0;

        public void DoUpdate(float deltaTime){
            if (!IsRunning) {
                return;
            }

            if (_constStateService.IsVideoMode) {
                return;
            }

            remainTime += deltaTime;
            if (_commonStateService.IsPause) {
                return;
            }

            while (remainTime >= 0.03f) {
                remainTime -= 0.03f;
                Step();
            }
        }

        private void Step(){
            if (_constStateService.IsClientMode) {
                var input = new Msg_PlayerInput(_world.Tick, _localActorId, _inputService.GetInputCmds());
                var frame = new ServerFrame() {
                    tick = _world.Tick,
                    _inputs = new Msg_PlayerInput[] {input}
                };
                _cmdBuffer.PushLocalFrame(frame);
                _cmdBuffer.PushServerFrames(new ServerFrame[]{frame});
                Simulate(_cmdBuffer.GetFrame(_world.Tick));
                return;
            }

            //cmdBuffer.Ping = _gameMsgService.Ping;
            _cmdBuffer.UpdateFramesInfo();
            var missFrameTick = _cmdBuffer.GetMissServerFrameTick();
            //客户端落后服务器太多帧 请求丢失帧
            if (_cmdBuffer.IsNeedReqMissFrame()) {
                _networkService.SendMissFrameReq(missFrameTick);
            }

            //if (!cmdBuffer.CanExecuteNextFrame()) { //因为网络问题 需要等待服务器发送确认包 才能继续往前
            //    return;
            //}
            _frameDeadline = LTime.realtimeSinceStartup + MaxSimulationMsPerFrame;

            var minTickToBackup = missFrameTick - FrameBuffer.SnapshotFrameInterval;
            //追帧 无输入
            var isPursueServer = !PursueServer(minTickToBackup);
            if (isPursueServer) {
                _constStateService.IsPursueFrame = true;
                Debug.Log($"PurchaseServering curTick:" + _world.Tick);
                EventHelper.Trigger(EEvent.PursueFrameProcess, GetPursueProgress());
                return;
            }

            if (_constStateService.IsPursueFrame) {
                EventHelper.Trigger(EEvent.PursueFrameDone);
            }

            _constStateService.IsPursueFrame = false;

            var frameDeltaTime = (LTime.realtimeSinceStartup - _timestampOnPurcue) * 1000;
            var targetTick = (float) System.Math.Ceiling(frameDeltaTime / NetworkDefine.UPDATE_DELTATIME) +
                             _tickOnPursue;
            //正常跑帧
            while (_world.Tick < targetTick) {
                var curTick = _world.Tick;
                _cmdBuffer.UpdateFramesInfo();
                //校验服务器包  如果有预测失败 则需要进行回滚
                if (_cmdBuffer.IsNeedRevert) {
                    _world.RollbackTo(_cmdBuffer.nextTickToCheck, missFrameTick);
                    _world.CleanUselessSnapshot(System.Math.Min(_cmdBuffer.nextTickToCheck - 1, _world.Tick));

                    minTickToBackup = System.Math.Max(minTickToBackup, _world.Tick + 1);
                    while (_world.Tick < missFrameTick) {
                        var sFrame = _cmdBuffer.GetServerFrame(_world.Tick);
                        Logging.Debug.Assert(sFrame != null && sFrame.tick == _world.Tick,
                            $" logic error: server Frame  must exist tick {_world.Tick}");
                        //服务器超前 客户端 应该追上去 将服务器中的输入作为客户端输入
                        _cmdBuffer.PushLocalFrame(sFrame);
                        Simulate(sFrame, _world.Tick >= minTickToBackup);
                    }

                    while (_world.Tick < curTick) {
                        var frame = _cmdBuffer.GetLocalFrame(_world.Tick);
                        FillInputWithLastFrame(frame); //加上输入预判 减少回滚
                        Logging.Debug.Assert(frame != null && frame.tick == _world.Tick,
                            $" logic error: local frame must exist tick {_world.Tick}");
                        Predict(frame, _world.Tick > minTickToBackup);
                    }
                }

                {
                    if (_world.Tick == curTick) { //当前帧 没有被执行 需要执行之
                        ServerFrame cFrame = null;
                        var sFrame = _cmdBuffer.GetServerFrame(_world.Tick);
                        if (sFrame != null) {
                            cFrame = sFrame;
                        }
                        else {
                            var input = new Msg_PlayerInput(curTick, _localActorId, _inputService.GetInputCmds());
                            cFrame = new ServerFrame();
                            var inputs = new Msg_PlayerInput[_actorCount];
                            inputs[_localActorId] = input;
                            cFrame.Inputs = inputs;
                            cFrame.tick = curTick;
                            FillInputWithLastFrame(cFrame);
                            if (curTick > _cmdBuffer.maxServerTickInBuffer) { //服务器的输入还没到 需要同步输入到服务器
                                SendInput(input);
                            }
                        }

                        _cmdBuffer.PushLocalFrame(cFrame);
                        Predict(cFrame);
                    }
                }
            } //end of while(_world.Tick < targetTick)

            CheckAndSendHashCodes();
        }


        private void SendInput(Msg_PlayerInput input){
            //TODO 合批次 一起发送 且连同历史未确认包一起发送
            if (!_constStateService.IsClientMode) {
                _networkService.SendInput(input);
            }
        }


        public void JumpTo(int tick){
            if (tick + 1 == _world.Tick || tick == _world.Tick) return;
            tick = LMath.Min(tick, _videoFrames.frames.Length - 1);
            var time = LTime.realtimeSinceStartup + 0.05f;
            if (!_isInitVideo) {
                _constStateService.IsVideoLoading = true;
                while (_world.Tick < _videoFrames.frames.Length) {
                    var sFrame = _videoFrames.frames[_world.Tick];
                    Simulate(sFrame, true);
                    if (LTime.realtimeSinceStartup > time) {
                        EventHelper.Trigger(EEvent.VideoLoadProgress, _world.Tick * 1.0f / _videoFrames.frames.Length);
                        return;
                    }
                }

                _constStateService.IsVideoLoading = false;
                EventHelper.Trigger(EEvent.VideoLoadDone);
                _isInitVideo = true;
            }

            if (_world.Tick > tick) {
                _world.RollbackTo(tick, _videoFrames.frames.Length, false);
            }

            while (_world.Tick <= tick) {
                var sFrame = _videoFrames.frames[_world.Tick];
                Simulate(sFrame, false);
            }

            _viewService.RebindAllEntities();
            _timestampOnLastJumpTo = LTime.timeSinceLevelLoad;
            _tickOnLastJumpTo = tick;
        }


        public void RunVideo(){
            if (_tickOnLastJumpTo == _world.Tick) {
                _timestampOnLastJumpTo = LTime.realtimeSinceStartup;
                _tickOnLastJumpTo = _world.Tick;
            }

            var frameDeltaTime = (LTime.timeSinceLevelLoad - _timestampOnLastJumpTo) * 1000;
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

        private void Simulate(ServerFrame frame, bool isNeedGenSnap = true){
            ProcessInputQueue(frame);
            _world.Simulate(isNeedGenSnap);
            var tick = _world.Tick;
            _cmdBuffer.SetClientTick(tick);
            SetHashCode();
            if (isNeedGenSnap && tick % FrameBuffer.SnapshotFrameInterval == 0) {
                _world.CleanUselessSnapshot(System.Math.Min(_cmdBuffer.nextTickToCheck - 1, _world.Tick));
            }
        }

        private void Predict(ServerFrame frame, bool isNeedGenSnap = true){
            ProcessInputQueue(frame);
            _world.Predict(isNeedGenSnap);
            var tick = _world.Tick;
            _cmdBuffer.SetClientTick(tick);
            SetHashCode();
            //清理无用 snapshot
            if (isNeedGenSnap && tick % FrameBuffer.SnapshotFrameInterval == 0) {
                _world.CleanUselessSnapshot(System.Math.Min(_cmdBuffer.nextTickToCheck - 1, _world.Tick));
            }
        }

        public void OnGameCreate(int targetFps, byte localActorId, byte actorCount, bool isNeedRender = true){
            FrameBuffer.DebugMainActorID = localActorId;
            var allActors = new byte[actorCount];
            for (byte i = 0; i < actorCount; i++) {
                allActors[i] = i;
            }

            //初始化全局配置

            _localActorId = localActorId;
            _allActors = allActors;

            _localTick = 0;

            _actorCount = allActors.Length;
            _tickDt = 1000f / targetFps;

            _playerInputs.Clear();
            for (int i = 0; i < actorCount; i++) {
                _playerInputs.Add(new PlayerInput());
            }

            _world.StartSimulate(_serviceContainer, _mgrContainer);
            EventHelper.Trigger(EEvent.LevelLoadProgress, 1f);
        }

        private void FillInputWithLastFrame(ServerFrame frame){
            int tick = frame.tick;
            var inputs = frame.Inputs;
            var lastServerFrames = tick == 0 ? null : _cmdBuffer.GetFrame(tick - 1)?.Inputs;
            var curServerFrame = inputs[_localActorId];
            //将所有角色 给予默认的输入
            for (int i = 0; i < _actorCount; i++) {
                inputs[i] = new Msg_PlayerInput(tick, _allActors[i], lastServerFrames?[i]?.Commands?.ToList());
            }

            inputs[_localActorId] = curServerFrame;
        }


        public void CheckAndSendHashCodes(){
            if (_cmdBuffer.nextTickToCheck > _firstHashTick) {
                var count = LMath.Min(allHashCodes.Count, (int) (_cmdBuffer.nextTickToCheck - _firstHashTick),
                    (480 / 8));
                if (count > 0) {
                    _networkService.SendHashCodes(_firstHashTick, allHashCodes, 0, count);
                    _firstHashTick = _firstHashTick + count;
                    allHashCodes.RemoveRange(0, count);
                }
            }
        }

        public void SetHash(int tick, long hash){
            if (tick < _firstHashTick) {
                return;
            }

            var idx = (int) (tick - _firstHashTick);
            if (allHashCodes.Count <= idx) {
                for (int i = 0; i < idx + 1; i++) {
                    allHashCodes.Add(0);
                }
            }

            allHashCodes[idx] = hash;
        }

        public void SetHashCode(){
            var nextTick = _world.Tick;
            var iTick = (int) nextTick - 1;
            for (int i = allHashCodes.Count; i <= iTick; i++) {
                allHashCodes.Add(0);
            }

            var hash = _world.HashCode;
            allHashCodes[iTick] = hash;
            SetHash(nextTick - 1, hash);
        }

        private void ProcessInputQueue(ServerFrame frame){
            var inputs = frame.Inputs;
            foreach (var playerInput in _playerInputs) {
                playerInput.Reset();
            }

            foreach (var input in inputs) {
                if (input.Commands == null) continue;
                foreach (var command in input.Commands) {
                    Logger.Trace(this, input.ActorId + " >> " + input.Tick + ": " + input.Commands.Count());
                    var inputEntity = _playerInputs[input.ActorId];
                    _inputService.Execute(command, inputEntity);
                }
            }
        }
    }
}