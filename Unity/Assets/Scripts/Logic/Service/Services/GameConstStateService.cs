using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {
    public partial class GameConstStateService : BaseService, IGameConstStateService {
        public static GameConstStateService Instance { get; private set; }

        public GameConstStateService(){
            Instance = this;
        }

        public bool IsPlaying { get; set; }

        public int MaxPlayerCount => 2;

        //room info
        public byte[] _allActorIds;
        public int _roomId;
        public int _actorCount;
        public int _playerInitLifeCount;
        public LVector2Int _mapMin;
        public LVector2Int _mapMax;


        public byte[] allActorIds {
            get { return _allActorIds; }
            set { _allActorIds = value; }
        }

        public int roomId {
            get => _roomId;
            set => _roomId = value;
        }

        public int actorCount {
            get => _actorCount;
            set => _actorCount = value;
        }

        public int playerInitLifeCount {
            get => _playerInitLifeCount;
            set => _playerInitLifeCount = value;
        }

        public LVector2Int mapMin {
            get => _mapMin;
            set => _mapMin = value;
        }

        public LVector2Int mapMax {
            get => _mapMax;
            set => _mapMax = value;
        }


        public List<LVector2> _playerBornPoss;

        public List<LVector2> enemyBornPoints {
            get { return _enemyBornPoints; }
            set { _enemyBornPoints = value; }
        }

        public List<LVector2> _enemyBornPoints;

        public List<LVector2> playerBornPoss {
            get { return _playerBornPoss; }
            set { _playerBornPoss = value; }
        }

        public LVector2 campPos { get; set; }


        public int MaxEnemyCountInScene { get; set; }
        public int TotalEnemyCountToBorn { get; set; }

        public bool IsGameOver;

        public override void DoStart(){
            base.DoStart();
            playerInitLifeCount = 3;
            IsGameOver = false;
        }


        public void OnEvent_LevelLoadDone(object param){
            var level = (int) param;
            IsGameOver = false;
            _constStateService.CurLevel = level;
        }

        public void OnEvent_SimulationStart(object param){
            IsPlaying = true;
        }

        private void GameFalied(){
            IsGameOver = true;
            ShowMessage("Game Over!!");
        }

        private void GameWin(){
            IsGameOver = true;
            //f (CurLevel >= MAX_LEVEL_COUNT) {
            //   ShowMessage("You Win!!");
            //
            //lse {
            //   //Map2DService.Instance.LoadLevel(CurLevel + 1);
            //
        }

        void ShowMessage(string msg){ }
    }
}