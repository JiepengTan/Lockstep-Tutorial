
using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using LockstepTutorial;

namespace Lockstep.Game {

    public partial class GameStateService: BaseGameService, IGameStateService {
        public class CopyStateCmd : BaseCommand {
            private GameState state;

            public override void Do(object param){
                state = ((GameStateService) param)._curGameState;
            }

            public override void Undo(object param){
                ((GameStateService) param)._curGameState = state;
            }
        }

        
        
        private GameState _curGameState;
        
        private Dictionary<Type, IList> _type2Entities = new Dictionary<Type, IList>();

        public void AddEntity<T>(T e) where T : class{
            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj)) {
                var lst = lstObj as List<T>;
                lst.Add(e);
            }
            else {
                var lst = new List<T>();
                _type2Entities.Add(t, lst);
                lst.Add(e);
            }
        }

        public void RemoveEntity<T>(T e) where T : class{
            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj)) {
                var lst = lstObj as List<T>;
                lst.Remove(e);
            }
            else {
                Debug.LogError("Try remove a deleted Entity" + e);
            }
        }

        public List<T> GetEntities<T>(){
            var t = typeof(T);
            if (_type2Entities.TryGetValue(t, out var lstObj)) {
                return lstObj as List<T>;
            }
            else {
                var lst = new List<T>();
                _type2Entities.Add(t, lst);
                return lst;
            }
        }

        public List<Enemy> GetEnemies(){return GetEntities<Enemy>();}
        public List<Player> GetPlayers(){return GetEntities<Player>();}
        public List<Spawner> GetSpawners(){return GetEntities<Spawner>();}
        
        
        public override void Backup(int tick){
            cmdBuffer.Execute(tick, new CopyStateCmd());
        }

        protected override FuncUndoCommands GetRollbackFunc(){
            return (minTickNode, maxTickNode, param) => { minTickNode.cmd.Undo(param); };
        }

        public struct GameState {
            public LFloat RemainTime;
            public LFloat DeltaTime;
        }

        public LFloat RemainTime {
            get => _curGameState.RemainTime;
            set => _curGameState.RemainTime = value;
        }
        public LFloat DeltaTime {
            get => _curGameState.DeltaTime;
            set => _curGameState.DeltaTime = value;
        }
    }
}