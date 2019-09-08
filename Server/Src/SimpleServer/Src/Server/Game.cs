//#define DEBUG_SHOW_INPUT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lockstep.Game;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Serialization;
using NetMsg.Common;
using Lockstep.Util;
using Random = System.Random;

#if DEBUG_SHOW_INPUT
namespace Lockstep.Game {
    public partial class PlayerInput : BaseFormater, IComponent {
        public LVector2 mousePos;
        public LVector2 inputUV;
        public bool isInputFire;
        public int skillId;
        public bool isSpeedUp;

        public override void Serialize(Serializer writer){
            writer.Write(mousePos);
            writer.Write(inputUV);
            writer.Write(isInputFire);
            writer.Write(skillId);
            writer.Write(isSpeedUp);
        }

        public void Reset(){
            mousePos = LVector2.zero;
            inputUV = LVector2.zero;
            isInputFire = false;
            skillId = 0;
            isSpeedUp = false;
        }

        public override void Deserialize(Deserializer reader){
            mousePos = reader.ReadLVector2();
            inputUV = reader.ReadLVector2();
            isInputFire = reader.ReadBoolean();
            skillId = reader.ReadInt32();
            isSpeedUp = reader.ReadBoolean();
        }

        public static PlayerInput Empty = new PlayerInput();

        public override bool Equals(object obj){
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as PlayerInput;
            return Equals(other);
        }

        public bool Equals(PlayerInput other){
            if (other == null) return false;
            if (mousePos != other.mousePos) return false;
            if (inputUV != other.inputUV) return false;
            if (isInputFire != other.isInputFire) return false;
            if (skillId != other.skillId) return false;
            if (isSpeedUp != other.isSpeedUp) return false;
            return true;
        }

        public PlayerInput Clone(){
            var tThis = this;
            return new PlayerInput() {
                mousePos = tThis.mousePos,
                inputUV = tThis.inputUV,
                isInputFire = tThis.isInputFire,
                skillId = tThis.skillId,
                isSpeedUp = tThis.isSpeedUp,
            };
        }
    }
}
#endif
namespace Lockstep.FakeServer {
    public class Game : BaseLogger {
        private delegate void DealNetMsg(Player player, BaseMsg data);

        private delegate BaseMsg ParseNetMsg(Deserializer reader);


        public const int MaxPlayerCount = 2;

        public int MapId { get; set; }
        public string GameHash { get; set; }

        public IPEndInfo TcpEnd { get; set; }
        public IPEndInfo UdpEnd { get; set; }

        public EGameState State = EGameState.Idle;
        public int GameType { get; set; }
        public int GameId { get; set; }

        public long[] UserIds => _userId2LocalId.Keys.ToArray();

        public int CurPlayerCount {
            get {
                int count = 0;
                foreach (var player in Players) {
                    if (player != null) {
                        count++;
                    }
                }

                return count;
            }
        }


        public bool IsRunning { get; private set; }
        public string Name;
        public float TimeSinceCreate;
        public bool IsFinished = false;

        public Msg_G2C_GameStartInfo GameStartInfo { get; set; }


        private Dictionary<long, byte> _userId2LocalId = new Dictionary<long, byte>();
        public Player[] Players { get; private set; }


        //hashcode 
        public int Tick = 0;
        private Dictionary<int, HashCodeMatcher> _hashCodes = new Dictionary<int, HashCodeMatcher>();


        private const int MaxMsgIdx = (short) EMsgSC.EnumCount;
        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[MaxMsgIdx];
        private ParseNetMsg[] allMsgParsers = new ParseNetMsg[MaxMsgIdx];


        private float _timeSinceLoaded;
        private float _firstFrameTimeStamp = 0;
        private float _waitTimer = 0;

        //所有需要等待输入到来的Ids
        private List<byte> _allNeedWaitInputPlayerIds;
        private List<ServerFrame> _allHistoryFrames = new List<ServerFrame>(); //所有的历史帧

        private byte[] _playerLoadingProgress;
        public const int MaxRepMissFrameCountPerPack = 600;

        public int Seed { get; set; }

        public void OnRecvPlayerGameData(Player player){
            if (player == null || MaxPlayerCount <= player.LocalId || Players[player.LocalId] != player) {
                return;
            }

            bool hasRecvAll = true;
            foreach (var user in Players) {
                if (user != null && user.GameData == null) {
                    hasRecvAll = false;
                    break;
                }
            }

            var playerCount = MaxPlayerCount;
            if (hasRecvAll) {
                //TODO 
                for (int i = 0; i < playerCount; i++) {
                    var helloMsg = new Msg_G2C_Hello() {
                        LocalId = (byte) i
                    };
                    Players[i].SendTcp(EMsgSC.G2C_Hello, helloMsg);
                }

                var userInfos = new GameData[playerCount];
                for (int i = 0; i < playerCount; i++) {
                    userInfos[i] = Players[i]?.GameData;
                }

                //all user data ready notify game start
                SetStartInfo(new Msg_G2C_GameStartInfo() {
                    MapId = MapId,
                    RoomId = GameId,
                    Seed = Seed,
                    UserCount = MaxPlayerCount,
                    TcpEnd = TcpEnd,
                    UdpEnd = UdpEnd,
                    SimulationSpeed = 30,
                    UserInfos = userInfos
                });
            }
        }


        public int GetUserLocalId(long userId){
            if (_userId2LocalId.TryGetValue(userId, out var id)) {
                return id;
            }

            return -1;
        }


        #region  life cycle

        public void DoStart(int gameId, int gameType, int mapId, Player[] playerInfos, string gameHash){
            State = EGameState.Loading;
            Seed = LRandom.Range(1, 100000);
            Tick = 0;
            _timeSinceLoaded = 0;
            _firstFrameTimeStamp = 0;
            RegisterMsgHandlers();
            Debug = new DebugInstance("Room" + GameId + ": ");
            var count = playerInfos.Length;
            GameType = gameType;
            GameHash = gameHash;
            GameId = gameId;
            Name = gameId.ToString();
            MapId = mapId;
            Players = playerInfos;
            _userId2LocalId.Clear();
            TimeSinceCreate = LTime.timeSinceLevelLoad;
            for (byte i = 0; i < count; i++) {
                var player = Players[i];
                _userId2LocalId.Add(player.UserId, player.LocalId);
            }

            //Temp code 
            for (byte i = 0; i < count; i++) {
                var player = Players[i];
                player.GameData = new GameData();
                OnRecvPlayerGameData(player);
            }
        }

        public void DoUpdate(float deltaTime){
            _timeSinceLoaded += deltaTime;
            _waitTimer += deltaTime;
            if (State != EGameState.Playing) return;
            if (_gameStartTimestampMs <= 0) return;
            while (Tick < _tickSinceGameStart) {
                _CheckBorderServerFrame(true);
            }
        }

        public void DoDestroy(){
            Log($"Room {GameId} Destroy");
            DumpGameFrames();
        }

        Player CreatePlayer(GamePlayerInfo playerInfo, byte localId){
            var player = Pool.Get<Player>();
            player.UserId = playerInfo.UserId;
            player.Account = playerInfo.Account;
            player.LoginHash = playerInfo.LoginHash;
            player.LocalId = localId;
            player.Game = this;
            return player;
        }

        private bool _CheckBorderServerFrame(bool isForce = false){
            if (State != EGameState.Playing) return false;
            var frame = GetOrCreateFrame(Tick);
            var inputs = frame.Inputs;
            if (!isForce) {
                //是否所有的输入  都已经等到
                for (int i = 0; i < inputs.Length; i++) {
                    if (inputs[i] == null) {
                        return false;
                    }
                }
            }

            //将所有未到的包 给予默认的输入
            for (int i = 0; i < inputs.Length; i++) {
                if (inputs[i] == null) {
                    inputs[i] = new Msg_PlayerInput(Tick, (byte) i) {IsMiss = true};
                }
            }

            //Debug.Log($" Border input {Tick} isUpdate:{isForce} _tickSinceGameStart:{_tickSinceGameStart}");
            var msg = new Msg_ServerFrames();
            int count = Tick < 2 ? Tick + 1 : 3;
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[count - i - 1] = _allHistoryFrames[Tick - i];
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            BorderUdp(EMsgSC.G2C_FrameData, msg);
            if (_firstFrameTimeStamp <= 0) {
                _firstFrameTimeStamp = _timeSinceLoaded;
            }

            if (_gameStartTimestampMs < 0) {
                _gameStartTimestampMs =
                    LTime.realtimeSinceStartupMS + NetworkDefine.UPDATE_DELTATIME * _ServerTickDealy;
            }

            Tick++;
            return true;
        }


        private void DumpGameFrames(){
            var msg = new Msg_RepMissFrame();
            int count = System.Math.Min((Tick - 1), _allHistoryFrames.Count);
            if (count <= 0) return;
            var writer = new Serializer();
            GameStartInfo.Serialize(writer);
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[i] = _allHistoryFrames[i];
                Logging.Debug.Assert(frames[i] != null, "!!!!!!!!!!!!!!!!!");
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            msg.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "../Record/" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + GameType + "_" + GameId +
                ".record");
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            Log("Create Record " + path);
            File.WriteAllBytes(path, bytes);
        }

        #endregion

        #region net msg

        public void SetStartInfo(Msg_G2C_GameStartInfo info){
            Debug.Log("SetStartInfo");
            GameStartInfo = info;
            BorderTcp(EMsgSC.G2C_GameStartInfo, GameStartInfo);
        }

        public void OnRecvMsg(Player player, Deserializer reader){
            if (reader.IsEnd) {
                DealMsgHandlerError(player, $"{player.UserId} send a Error:Net Msg");
                return;
            }

            var msgType = reader.ReadInt16();
            if (msgType >= MaxMsgIdx) {
                DealMsgHandlerError(player, $"{player.UserId} send a Error msgType out of range {msgType}");
                return;
            }

            //Debug.Log($"OnDataReceived netID = {player.localId}  type:{(EMsgCS) msgType}");
            {
                var _func = allMsgDealFuncs[msgType];
                var _parser = allMsgParsers[msgType];
                if (_func != null && _parser != null) {
                    var data = _parser(reader);
                    if (data == null) {
                        DealMsgHandlerError(player,
                            $"ErrorMsg type :parser data error playerID = {player.UserId} msgType:{msgType}");
                        return;
                    }

                    _func(player, data);
                }
                else {
                    DealMsgHandlerError(player,
                        $" {player.UserId} ErrorMsg type :no msg handler or parser {msgType}");
                }
            }
        }

        void DealMsgHandlerError(Player player, string msg){
            LogError(msg);
            TickOut(player, 0);
        }

        public void TickOut(Player player, int reason){
            //_gameServer.TickOut(player, reason);
        }

        private void RegisterMsgHandlers(){
            RegisterHandler(EMsgSC.C2G_PlayerInput, C2G_PlayerInput,
                (reader) => { return ParseData<Msg_PlayerInput>(reader); });
            RegisterHandler(EMsgSC.C2G_PlayerPing, C2G_PlayerPing,
                (reader) => { return ParseData<Msg_C2G_PlayerPing>(reader); });
            RegisterHandler(EMsgSC.C2G_HashCode, C2G_HashCode,
                (reader) => { return ParseData<Msg_HashCode>(reader); });
            RegisterHandler(EMsgSC.C2G_LoadingProgress, C2G_LoadingProgress,
                (reader) => { return ParseData<Msg_C2G_LoadingProgress>(reader); });
            RegisterHandler(EMsgSC.C2G_ReqMissFrame, C2G_ReqMissFrame,
                (reader) => { return ParseData<Msg_ReqMissFrame>(reader); });
            RegisterHandler(EMsgSC.C2G_RepMissFrameAck, C2G_RepMissFrameAck,
                (reader) => { return ParseData<Msg_RepMissFrameAck>(reader); });
        }

        private void RegisterHandler(EMsgSC type, DealNetMsg func, ParseNetMsg parseFunc){
            allMsgDealFuncs[(int) type] = func;
            allMsgParsers[(int) type] = parseFunc;
        }

        T ParseData<T>(Deserializer reader) where T : BaseMsg, new(){
            T data = null;
            try {
                data = reader.Parse<T>();
                if (!reader.IsEnd) {
                    data = null;
                }
            }
            catch (Exception e) {
                LogError("Parse Msg Error:" + e);
                data = null;
            }

            return data;
        }

        public void BorderTcp(EMsgSC type, BaseMsg data){
            var bytes = data.ToBytes();
            foreach (var player in Players) {
                player?.SendTcp(type, bytes);
            }
        }

        public void BorderUdp(EMsgSC type, byte[] data){
            foreach (var player in Players) {
                SendUdp(player, type, data);
            }
        }

        public void BorderUdp(EMsgSC type, ISerializable body){
            var writer = new Serializer();
            body.Serialize(writer);
            var bytes = writer.CopyData();
            foreach (var player in Players) {
                player?.SendUdp(type, bytes);
            }
        }


        public void SendUdp(Player player, EMsgSC type, byte[] data){
            player?.SendUdp(type, data);
        }

        public void SendUdp(Player player, EMsgSC type, ISerializable body, bool isNeedDebugSize = false){
            var writer = new Serializer();
            body.Serialize(writer);
            player?.SendUdp(type, writer.CopyData());
        }

        #endregion

        #region net status

        public void OnPlayerConnect(Player player){
            if (GameStartInfo != null) {
                player.SendTcp(EMsgSC.G2C_GameStartInfo, GameStartInfo);
            }
        }

        //net status
        public void OnPlayerReconnect(GamePlayerInfo playerInfo){
            var localId = _userId2LocalId[playerInfo.UserId];
            var player = CreatePlayer(playerInfo, localId);
            Players[localId] = player;
        }

        public void OnPlayerReconnect(Player player){
            player.LocalId = _userId2LocalId[player.UserId];
            Players[player.LocalId] = player;
        }

        public void OnPlayerDisconnect(Player player){
            Log($"Player{player.UserId} OnDisconnect room {GameId}");
            RemovePlayer(player);
        }

        public void OnPlayerLeave(long userId){
            if (_userId2LocalId.TryGetValue(userId, out var localId)) {
                var player = Players[localId];
                if (player != null) {
                    OnPlayerLeave(player);
                }
            }
        }

        public void OnPlayerLeave(Player player){
            RemovePlayer(player);
            _userId2LocalId.Remove(player.UserId); //同时还需要彻底的移除记录 避免玩家重连
            Log($"Player{player.UserId} OnPlayerLeave room {GameId}");
        }

        void RemovePlayer(Player player){
            if (Players[player.LocalId] == null) return;
            Players[player.LocalId] = null;
            var peer = player.PeerTcp;
            peer?.Dispose();

            player.PeerTcp = null;
            peer = player.PeerUdp;
            peer?.Dispose();

            player.PeerUdp = null;

            var curCount = CurPlayerCount;
            if (curCount == 0) {
                Log("All players left, stopping current simulation...");
                IsRunning = false;
                State = EGameState.Idle;
                //_gameServer.OnGameEmpty(this);
            }
            else {
                Log(curCount + " players remaining.");
            }
        }

        #endregion

        #region game status

        public void StartGame(){ }

        public void FinishedGame(){ }

        #endregion

        #region IRecyclable

        //回收时候调用
        public void OnReuse(){ }

        public void OnRecycle(){
            _userId2LocalId.Clear();
            _hashCodes.Clear();
            Tick = 0;
            GameId = -1;
            if (Players == null) return;
            foreach (var player in Players) {
                Pool.Return(player);
            }

            Players = null;
        }

        #endregion

        #region Net msg handler

        public void OnNetMsg(Player player, ushort opcode, BaseMsg msg){
            var type = (EMsgSC) opcode;
            switch (type) {
                //ping 
                case EMsgSC.C2G_PlayerPing:
                    C2G_PlayerPing(player, msg);
                    break;
                //login
                //room
                case EMsgSC.C2G_PlayerInput:
                    C2G_PlayerInput(player, msg);
                    break;
                case EMsgSC.C2G_HashCode:
                    C2G_HashCode(player, msg);
                    break;
                case EMsgSC.C2G_LoadingProgress:
                    C2G_LoadingProgress(player, msg);
                    break;
                case EMsgSC.C2G_ReqMissFrame:
                    C2G_ReqMissFrame(player, msg);
                    break;
                case EMsgSC.C2G_RepMissFrameAck:
                    C2G_RepMissFrameAck(player, msg);
                    break;
                default:
                    Debug.Log("Unknow msg " + type);
                    break;
            }
        }

        public long _gameStartTimestampMs = -1;
        public int _ServerTickDealy = 0;

        public int _tickSinceGameStart =>
            (int) ((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);

        void C2G_PlayerPing(Player player, BaseMsg data){
            var msg = data as Msg_C2G_PlayerPing;
            player?.SendUdp(EMsgSC.G2C_PlayerPing, new Msg_G2C_PlayerPing() {
                localId = msg.localId,
                sendTimestamp = msg.sendTimestamp,
                timeSinceServerStart = LTime.realtimeSinceStartupMS - _gameStartTimestampMs
            });
        }

        void C2G_PlayerInput(Player player, BaseMsg data){
            if (State != EGameState.PartLoaded && State != EGameState.Playing) return;
            if (State == EGameState.PartLoaded) {
                Log("First input: game start playing");
                State = EGameState.Playing;
            }

            var input = data as Msg_PlayerInput;
#if DEBUG_SHOW_INPUT
            if (input.Commands != null && input.Commands?.Length > 0) {
                var cmd = input.Commands[0];
                var playerInput = new Deserializer(cmd.content).Parse<Lockstep.Game.PlayerInput>();
                if (playerInput.inputUV != LVector2.zero) {
                    Debug.Log(
                        $"curTick{Tick} isOutdate{input.Tick < Tick} RecvInput actorID:{input.ActorId}  inputTick:{input.Tick}  move:{playerInput.inputUV}");
                }
            }
#endif

            //Debug.Log($"RecvInput actorID:{input.ActorId} inputTick:{input.Tick} Tick{Tick}");
            if (input.Tick < Tick) {
                return;
            }

            var frame = GetOrCreateFrame(input.Tick);

            var id = input.ActorId;
            if (!_allNeedWaitInputPlayerIds.Contains(id)) {
                _allNeedWaitInputPlayerIds.Add(id);
            }

            frame.Inputs[id] = input;
            _CheckBorderServerFrame(false);
        }

        ServerFrame GetOrCreateFrame(int tick){
            //扩充帧队列
            var frameCount = _allHistoryFrames.Count;
            if (frameCount <= tick) {
                var count = tick - _allHistoryFrames.Count + 1;
                for (int i = 0; i < count; i++) {
                    _allHistoryFrames.Add(null);
                }
            }

            if (_allHistoryFrames[tick] == null) {
                _allHistoryFrames[tick] = new ServerFrame() {tick = tick};
            }

            var frame = _allHistoryFrames[tick];
            if (frame.Inputs == null || frame.Inputs.Length != MaxPlayerCount) {
                frame.Inputs = new Msg_PlayerInput[MaxPlayerCount];
            }

            return frame;
        }


        void C2G_HashCode(Player player, BaseMsg data){
            var hashInfo = data as Msg_HashCode;
            var id = player.LocalId;
            for (int i = 0; i < hashInfo.HashCodes.Length; i++) {
                var code = hashInfo.HashCodes[i];
                var tick = hashInfo.StartTick + i;
                //Debug.Log($"tick: {tick} Hash {code}"  );
                if (_hashCodes.TryGetValue(tick, out HashCodeMatcher matcher1)) {
                    if (matcher1 == null || matcher1.sendResult[id]) {
                        continue;
                    }

                    if (matcher1.hashCode != code) {
                        OnHashMatchResult(tick, code, false);
                    }

                    matcher1.count = matcher1.count + 1;
                    matcher1.sendResult[id] = true;
                    if (matcher1.IsMatchered) {
                        OnHashMatchResult(tick, code, true);
                    }
                }
                else {
                    var matcher2 = new HashCodeMatcher(MaxPlayerCount);
                    matcher2.count = 1;
                    matcher2.hashCode = code;
                    matcher2.sendResult[id] = true;
                    _hashCodes.Add(tick, matcher2);
                    if (matcher2.IsMatchered) {
                        OnHashMatchResult(tick, code, true);
                    }
                }
            }
        }


        void OnHashMatchResult(int tick, long hash, bool isMatched){
            if (isMatched) {
                _hashCodes[tick] = null;
            }

            if (!isMatched) {
                Log($"!!!!!!!!!!!! Hash not match tick{tick} hash{hash} ");
            }
        }


        void C2G_ReqMissFrame(Player player, BaseMsg data){
            var reqMsg = data as Msg_ReqMissFrame;
            var nextCheckTick = reqMsg.StartTick;
            Log($"C2G_ReqMissFrame nextCheckTick id:{player.LocalId}:{nextCheckTick}");
            int count = System.Math.Min(System.Math.Min((Tick - 1), _allHistoryFrames.Count) - nextCheckTick,
                MaxRepMissFrameCountPerPack);
            if (count <= 0) return;
            var msg = new Msg_RepMissFrame();
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[i] = _allHistoryFrames[nextCheckTick + i];
                Logging.Debug.Assert(frames[i] != null);
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            SendUdp(player, EMsgSC.G2C_RepMissFrame, msg, true);
        }

        void C2G_RepMissFrameAck(Player player, BaseMsg data){
            var msg = data as Msg_RepMissFrameAck;
            Log($"C2G_RepMissFrameAck missFrameTick:{msg.MissFrameTick}");
        }


        void C2G_LoadingProgress(Player player, BaseMsg data){
            if (State != EGameState.Loading) return;
            var msg = data as Msg_C2G_LoadingProgress;
            if (_playerLoadingProgress == null) {
                _playerLoadingProgress = new byte[MaxPlayerCount];
            }

            _playerLoadingProgress[player.LocalId] = msg.Progress;

            //Log($"palyer{player.LocalId} Load {msg.Progress}");

            BorderTcp(EMsgSC.G2C_LoadingProgress, new Msg_G2C_LoadingProgress() {
                Progress = _playerLoadingProgress
            });

            if (msg.Progress < 100) return;
            var isDone = true;
            foreach (var progress in _playerLoadingProgress) {
                if (progress < 100) {
                    isDone = false;
                    break;
                }
            }

            if (isDone) {
                OnFinishedLoaded();
            }
        }


        void OnFinishedLoaded(){
            Log("All Load done");
            State = EGameState.PartLoaded;
            for (int i = 0; i < _playerLoadingProgress.Length; i++) {
                _playerLoadingProgress[i] = 0;
            }

            _allNeedWaitInputPlayerIds = new List<byte>();
            foreach (var val in _userId2LocalId.Values) {
                _allNeedWaitInputPlayerIds.Add(val);
            }

            //BorderTcp(EMsgSC.G2C_GameStartInfo, GameStartInfo);
            BorderTcp(EMsgSC.G2C_AllFinishedLoaded, new Msg_G2C_AllFinishedLoaded() { });
        }

        #endregion
    }
}