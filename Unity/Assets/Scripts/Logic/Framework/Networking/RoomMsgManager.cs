using System;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
using Msg_HashCode = NetMsg.Common.Msg_HashCode;
using Msg_PlayerInput = NetMsg.Common.Msg_PlayerInput;

public interface IIncommingMessage {
    T Parse<T>();
    byte[] GetRawBytes();
}

namespace Lockstep.Game {
    public interface IRoomMsgManager {
        void Init(IRoomMsgHandler msgHandler);
        void SendInput(Msg_PlayerInput msg);
        void SendMissFrameReq(int missFrameTick);
        void SendMissFrameRepAck(int missFrameTick);
        void SendHashCodes(int firstHashTick, List<int> allHashCodes, int startIdx, int count);

        void SendGameEvent(byte[] data);
        void SendLoadingProgress(byte progress);


        void ConnectToGameServer(Msg_C2G_Hello helloBody, IPEndInfo _gameTcpEnd, bool isReconnect);
        void OnLevelLoadProgress(float progress);
    }

    public class RoomMsgManager : IRoomMsgManager {
        private delegate void DealNetMsg(BaseMsg data);

        private delegate BaseMsg ParseNetMsg(Deserializer reader);

        public EGameState CurGameState = EGameState.Idle;

        private NetClient _netUdp;
        private NetClient _netTcp;

        private float _curLoadProgress;
        private float _framePursueRate;

        public float FramePursueRate {
            get { return _framePursueRate; }
            set { _framePursueRate = System.Math.Max(System.Math.Min(1f, value), 0f); }
        }

        private float _nextSendLoadProgressTimer;
        private IRoomMsgHandler _handler;


        protected string _gameHash;
        protected int _curMapId;
        protected byte _localId;
        protected int _roomId;

        protected IPEndInfo _gameUdpEnd;
        protected IPEndInfo _gameTcpEnd;
        protected MessageHello helloBody;

        protected bool HasConnGameTcp;
        protected bool HasConnGameUdp;
        protected bool HasRecvGameDta;
        protected bool HasFinishedLoadLevel;

        public void Init(IRoomMsgHandler msgHandler){
            _maxMsgId = (byte) System.Math.Min((int) EMsgSC.EnumCount, (int) byte.MaxValue);
            _allMsgDealFuncs = new DealNetMsg[_maxMsgId];
            _allMsgParsers = new ParseNetMsg[_maxMsgId];
            RegisterMsgHandlers();
            _handler = msgHandler;
            _netUdp = _netTcp = new NetClient();//TODO Login
            _netTcp.DoStart();
            _netTcp.NetMsgHandler = OnNetMsg;
        }

        void OnNetMsg(ushort opcode, object msg){
            var type = (EMsgSC) opcode;
            switch (type) {
                //login
               // case EMsgSC.L2C_JoinRoomResult: 

                //room
                case EMsgSC.G2C_PlayerPing:    G2C_PlayerPing(msg); break;
                case EMsgSC.G2C_Hello:    G2C_Hello(msg); break;
                case EMsgSC.G2C_FrameData:    G2C_FrameData(msg); break;
                case EMsgSC.G2C_RepMissFrame:  G2C_RepMissFrame(msg); break;
                case EMsgSC.G2C_GameEvent:   G2C_GameEvent(msg); break;  
                case EMsgSC.G2C_GameStartInfo: G2C_GameStartInfo(msg); break;
                case EMsgSC.G2C_LoadingProgress:  G2C_LoadingProgress(msg); break;
                case EMsgSC.G2C_AllFinishedLoaded: G2C_AllFinishedLoaded(msg); break;

             }
        }

        public void DoUpdate(LFloat deltaTime){
            if (CurGameState == EGameState.Loading) {
                if (_nextSendLoadProgressTimer < Time.realtimeSinceStartup) {
                    SendLoadingProgress(CurProgress);
                }
            }
        }
        public void DoDestroy(){
            Debug.Log("DoDestroy");
            _netTcp.SendMessage(EMsgSC.C2L_LeaveRoom,new Msg_C2L_LeaveRoom().ToBytes() );
            _netUdp?.DoDestroy();
            _netTcp?.DoDestroy();
            _netTcp = null;
            _netUdp = null;
        }
        void ResetStatus(){
            HasConnGameTcp = false;
            HasConnGameUdp = false;
            HasRecvGameDta = false;
            HasFinishedLoadLevel = false;
        }

        public byte CurProgress {
            get {
                if (_curLoadProgress > 1) _curLoadProgress = 1;
                if (_curLoadProgress < 0) _curLoadProgress = 0;
                if (IsReconnecting) {
                    var val = (HasRecvGameDta ? 10 : 0) +
                              (HasConnGameUdp ? 10 : 0) +
                              (HasConnGameTcp ? 10 : 0) +
                              _curLoadProgress * 20 +
                              FramePursueRate * 50
                        ;
                    return (byte) val;
                }
                else {
                    var val = _curLoadProgress * 70 +
                              (HasRecvGameDta ? 10 : 0) +
                              (HasConnGameUdp ? 10 : 0) +
                              (HasConnGameTcp ? 10 : 0);

                    return (byte) val;
                }
            }
        }

        public const float ProgressSendInterval = 0.3f;
        public void OnLevelLoadProgress(float progress){
            _curLoadProgress = progress;
            if (CurProgress >= 100) {
                CurGameState = EGameState.PartLoaded;
                _nextSendLoadProgressTimer = Time.realtimeSinceStartup + ProgressSendInterval;
                SendLoadingProgress(CurProgress);
            }
        }

        public bool IsReconnecting { get; set; }

        public void ConnectToGameServer(Msg_C2G_Hello helloBody, IPEndInfo _gameTcpEnd, bool isReconnect){
            IsReconnecting = isReconnect;
            ResetStatus();
            this.helloBody = helloBody.Hello;
            ConnectUdp();
            //TODO temp code
            SendTcp(EMsgSC.C2L_JoinRoom,new Msg_C2L_JoinRoom() {
                RoomId = 0
            });
        }

        void ConnectUdp(){
            _handler.OnUdpHello(_curMapId, _localId);
        }


        #region tcp

        public Msg_G2C_GameStartInfo GameStartInfo { get; private set; }
        protected void G2C_PlayerPing(object reader){
            var msg = reader as Msg_G2C_PlayerPing;
            EventHelper.Trigger(EEvent.OnPlayerPing, msg);
        }
        protected void G2C_Hello(object reader){
            var msg = reader as Msg_G2C_Hello;
            EventHelper.Trigger(EEvent.OnServerHello, msg);
        }
        protected void G2C_GameEvent(object reader){
            var msg = reader as Msg_G2C_GameEvent;
            _handler.OnGameEvent(msg.Data);
        }

        protected void G2C_GameStartInfo(object reader){
            var msg = reader as Msg_G2C_GameStartInfo;
            HasRecvGameDta = true;
            GameStartInfo = msg;
            _handler.OnGameStartInfo(msg);
            //TODO temp code 
            HasConnGameTcp = true;
            HasConnGameUdp = true;
            CurGameState = EGameState.Loading;
            _curLoadProgress = 1;
            EventHelper.Trigger(EEvent.OnGameCreate, msg);
            Debug.Log("G2C_GameStartInfo");
        }

        private short curLevel;

        protected void G2C_LoadingProgress(object reader){
            var msg = reader as Msg_G2C_LoadingProgress;
            _handler.OnLoadingProgress(msg.Progress);
        }

        protected void G2C_AllFinishedLoaded(object reader){
            var msg = reader as Msg_G2C_AllFinishedLoaded;
            curLevel = msg.Level;
            _handler.OnAllFinishedLoaded(msg.Level);
        }

        public void SendGameEvent(byte[] msg){
            SendTcp(EMsgSC.C2G_GameEvent, new Msg_C2G_GameEvent() {Data = msg});
        }

        public void SendLoadingProgress(byte progress){
            _nextSendLoadProgressTimer = Time.realtimeSinceStartup + ProgressSendInterval;
            if (!IsReconnecting) {
                SendTcp(EMsgSC.C2G_LoadingProgress, new Msg_C2G_LoadingProgress() {
                    Progress = progress
                });
            }
        }

        #endregion

        #region udp

        private byte _maxMsgId = byte.MaxValue;
        private DealNetMsg[] _allMsgDealFuncs;
        private ParseNetMsg[] _allMsgParsers;


        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgSC.G2C_RepMissFrame, G2C_RepMissFrame, ParseData<Msg_RepMissFrame>);
            RegisterNetMsgHandler(EMsgSC.G2C_FrameData, G2C_FrameData, ParseData<Msg_ServerFrames>);
        }

        private void RegisterNetMsgHandler(EMsgSC type, DealNetMsg func, ParseNetMsg parseFunc){
            _allMsgDealFuncs[(int) type] = func;
            _allMsgParsers[(int) type] = parseFunc;
        }

        private T ParseData<T>(Deserializer reader) where T : BaseMsg, new(){
            return reader.Parse<T>();
        }

        public void SendPing(byte localId, long timestamp){
            SendUdp(EMsgSC.C2G_PlayerPing, new Msg_C2G_PlayerPing(){localId = localId,sendTimestamp =  timestamp});
        }
        public void SendInput(Msg_PlayerInput msg){
            SendUdp(EMsgSC.C2G_PlayerInput, msg);
        }
        
        public void SendMissFrameReq(int missFrameTick){
            SendUdp(EMsgSC.C2G_ReqMissFrame, new Msg_ReqMissFrame() {StartTick = missFrameTick});
        }

        public void SendMissFrameRepAck(int missFrameTick){
            SendUdp(EMsgSC.C2G_RepMissFrameAck, new Msg_RepMissFrameAck() {MissFrameTick = missFrameTick});
        }

        public void SendHashCodes(int firstHashTick, List<int> allHashCodes, int startIdx, int count){
            Msg_HashCode msg = new Msg_HashCode();
            msg.StartTick = firstHashTick;
            msg.HashCodes = new int[count];
            for (int i = startIdx; i < count; i++) {
                msg.HashCodes[i] = allHashCodes[i];
            }

            SendUdp(EMsgSC.C2G_HashCode, msg);
        }


        public void SendUdp(EMsgSC msgId, ISerializable body){
            var writer = new Serializer();
            body.Serialize(writer);
            _netUdp?.SendMessage(msgId, writer.CopyData());
        }

        public void SendTcp(EMsgSC msgId, BaseMsg body){
            var writer = new Serializer();
            body.Serialize(writer);
            _netTcp?.SendMessage(msgId, writer.CopyData());
        }

        protected void G2C_UdpMessage(IIncommingMessage reader){
            var bytes = reader.GetRawBytes();
            var data = new Deserializer(Compressor.Decompress(bytes));
            OnRecvMsg(data);
        }

        protected void OnRecvMsg(Deserializer reader){
            var msgType = reader.ReadInt16();
            if (msgType >= _maxMsgId) {
                Debug.LogError($" send a Error msgType out of range {msgType}");
                return;
            }

            try {
                var _func = _allMsgDealFuncs[msgType];
                var _parser = _allMsgParsers[msgType];
                if (_func != null && _parser != null) {
                    _func(_parser(reader));
                }
                else {
                    Debug.LogError($" ErrorMsg type :no msg handler or parser {msgType}");
                }
            }
            catch (Exception e) {
                Debug.LogError($" Deal Msg Error :{(EMsgSC) (msgType)}  " + e);
            }
        }

        protected void G2C_FrameData(object reader){
            var msg = reader as Msg_ServerFrames;
            _handler.OnServerFrames(msg);
        }

        protected void G2C_RepMissFrame(object reader){
            var msg = reader as Msg_RepMissFrame;
            _handler.OnMissFrames(msg);
        }

        #endregion
    }
}