
using Lockstep.Math;

namespace Lockstep.Game {

    public partial class GameStateService :BaseService, IGameStateService {
        public static GameStateService Instance { get; private set; }
        public GameStateService(){
            Instance = this;
        }
        
        private GameState curGameState = new GameState();

        public class CopyStateCmd : BaseCommand {
            private GameState state;

            public override void Do(object param){
                state = ((GameStateService) param).curGameState;
            }

            public override void Undo(object param){
                ((GameStateService) param).curGameState = state;
            }
        }

        public override void Backup(int tick){
            cmdBuffer.Execute(tick, new CopyStateCmd());
        }

        protected override FuncUndoCommands GetRollbackFunc(){
            return (minTickNode, maxTickNode, param) => { minTickNode.cmd.Undo(param); };
        }

        public struct GameState {
            public int CurEnemyCountInScene;
            public int RemainCountToBorn;
            public LFloat BornTimer;
            public LFloat BornInterval;
        }

        public int curEnemyCountInScene {
            get => curGameState.CurEnemyCountInScene;
            set => curGameState.CurEnemyCountInScene = value;
        }

        public int remainCountToBorn {
            get => curGameState.RemainCountToBorn;
            set => curGameState.RemainCountToBorn = value;
        }

        public LFloat bornTimer {
            get => curGameState.BornTimer;
            set => curGameState.BornTimer = value;
        }

        public LFloat bornInterval {
            get => curGameState.BornInterval;
            set => curGameState.BornInterval = value;
        }

        private LFloat _deltaTime = new LFloat(true, 16);

        public LFloat DeltaTime {
            get => _deltaTime;
            set => _deltaTime = value;
        }
    }
}