using System.Collections.Generic;
using Lockstep.Network;
using Lockstep.Logging;

namespace Lockstep.FakeServer {
    public class Room {
        public const int MaxPlayerCount = 2;
        // player infos
        private PlayerServerInfo[] _playerInfos;
        private Session[] _sessions;
        
        //map
        private Dictionary<int, int> _id2LocalId = new Dictionary<int, int>();
        private Dictionary<int, PlayerInput[]> _tick2Inputs = new Dictionary<int, PlayerInput[]>();
        private Dictionary<int, int[]> _tick2Hashes = new Dictionary<int, int[]>();
        
        //cur status
        private int _curLocalId;
        private int _curTick;
        
        public bool IsRunning;

        public void Init(int type){
            _playerInfos = new PlayerServerInfo[MaxPlayerCount];
            _sessions = new Session[MaxPlayerCount];
        }



        public void DoUpdate(float timeSinceStartUp, float deltaTime){
            if (!IsRunning) return;
            CheckInput();
        }


        private void CheckInput(){
            if (_tick2Inputs.TryGetValue(_curTick, out var inputs)) {
                if (inputs != null) {
                    bool isFullInput = true;
                    for (int i = 0; i < inputs.Length; i++) {
                        if (inputs[i] == null) {
                            isFullInput = false;
                            break;
                        }
                    }

                    if (isFullInput) {
                        BoardInputMsg(_curTick, inputs);
                        _tick2Inputs.Remove(_curTick);
                        _curTick++;
                    }
                }
            }
        }

        private void BoardInputMsg(int tick, PlayerInput[] inputs){
            var frame = new Msg_FrameInput();
            frame.input = new FrameInput() {
                tick = tick,
                inputs = inputs
            };
            var bytes = frame.ToBytes();
            for (int i = 0; i < MaxPlayerCount; i++) {
                var session = _sessions[i];
                session.Send((int) EMsgType.FrameInput, bytes);
            }
        }
        
        public void OnGameStart(){
            IsRunning = true;
            _curTick = 0;
            var frame = new Msg_StartGame();
            frame.mapId = 0;
            frame.playerInfos = _playerInfos;
            for (int i = 0; i < MaxPlayerCount; i++) {
                var session = _sessions[i];
                frame.localPlayerId = i;
                var bytes = frame.ToBytes();
                session.Send((int) EMsgType.StartGame, bytes);
            }
        }
        
        public void OnPlayerJoin(Session session, PlayerServerInfo player){
            if (_id2LocalId.ContainsKey(player.Id)) return;
            _id2LocalId[player.Id] = _curLocalId;
            _playerInfos[_curLocalId] = player;
            _sessions[_curLocalId] = session;
            _curLocalId++;
        }
        
        public void OnPlayerInput(int useId, Msg_PlayerInput msg){
            //Debug.Log($" Recv Input: {useId} {msg.input.inputUV}");
            int localId = 0;
            if (!_id2LocalId.TryGetValue(useId, out localId)) return;
            PlayerInput[] inputs;
            if (!_tick2Inputs.TryGetValue(msg.tick, out inputs)) {
                inputs = new PlayerInput[MaxPlayerCount];
                _tick2Inputs.Add(msg.tick, inputs);
            }

            inputs[localId] = msg.input;
            CheckInput();
        }

        public void OnPlayerHashCode(int useId, Msg_HashCode msg){
            int localId = 0;
            if (!_id2LocalId.TryGetValue(useId, out localId)) return;
            int[] hashes;
            if (!_tick2Hashes.TryGetValue(msg.tick, out hashes)) {
                hashes = new int[MaxPlayerCount];
                _tick2Hashes.Add(msg.tick, hashes);
            }

            hashes[localId] = msg.hash;
            //check hash
            foreach (var hash in hashes) {
                if (hash == 0)
                    return;
            }

            bool isSame = true;
            var val = hashes[0];
            foreach (var hash in hashes) {
                if (hash != val) {
                    isSame = false;
                    break;
                }
            }

            if (!isSame) {
                Debug.Log(msg.tick + " Hash is different " + val);
            }

            _tick2Hashes.Remove(msg.tick);
        }
    }
}