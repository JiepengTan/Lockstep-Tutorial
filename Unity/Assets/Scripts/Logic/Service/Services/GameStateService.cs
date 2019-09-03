using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Game;
using Lockstep.Serialization;

namespace Lockstep.Game {
    public partial class GameStateService : BaseGameService, IGameStateService {
        private GameState _curGameState;
        private Dictionary<Type, IList> _type2Entities = new Dictionary<Type, IList>();
        private Dictionary<int, object> _id2Entities = new Dictionary<int, object>();
        Dictionary<int, Serializer> _tick2Backup = new Dictionary<int, Serializer>();


        public class CopyStateCmd : BaseCommand {
            private GameState _state;

            public override void Do(object param){
                _state = ((GameStateService) param)._curGameState;
            }

            public override void Undo(object param){
                ((GameStateService) param)._curGameState = _state;
            }
        }


        private void AddEntity<T>(T e) where T : class{
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

        private void RemoveEntity<T>(T e) where T : class{
            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj)) {
                var lst = lstObj as List<T>;
                lst.Remove(e);
            }
            else {
                Debug.LogError("Try remove a deleted Entity" + e);
            }
        }

        private List<T> GetEntities<T>(){
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

        public List<Enemy> GetEnemies(){
            return GetEntities<Enemy>();
        }

        public List<Player> GetPlayers(){
            return GetEntities<Player>();
        }

        public List<Spawner> GetSpawners(){
            return GetEntities<Spawner>();
        }


        public object GetEntity(int id){
            if (_id2Entities.TryGetValue(id, out var val)) {
                return val;
            }

            return null;
        }

        public void CreateEnemy(int prefabId, LVector3 position){
            if (CurEnemyCount >= MaxEnemyCount) {
                return;
            }

            CurEnemyCount++;
            var entity = _gameEntityService.CreateEnemy(prefabId, position) as Enemy;
            entity.OnDied += (e) => { RemoveEnemy(e as Enemy); };
            AddEnemy(entity);
        }


        public void AddEnemy(Enemy enemy){
            AddEntity(enemy);
        }

        public void RemoveEnemy(Enemy enemy){
            RemoveEntity(enemy);
        }
        public void AddPlayer(Player entity){
            AddEntity(entity);
        }
        public void RemovePlayer(Player entity){
            RemoveEntity(entity);
        }
        public override void Backup(int tick){
            //
            Serializer writer = new Serializer();
            BackUpEntities(GetEnemies(), writer);
            BackUpEntities(GetPlayers(), writer);
            BackUpEntities(GetSpawners(), writer);
            _tick2Backup[tick] = writer;

            cmdBuffer.Execute(tick, new CopyStateCmd());
        }

        public override void RollbackTo(int tick){
            base.RollbackTo(tick);
            if (_tick2Backup.TryGetValue(tick, out var backupData)) {
                var reader = new Deserializer(backupData.Data);
                var enemies = RecoverEntities(new List<Enemy>(), reader);
                var players = RecoverEntities(new List<Player>(), reader);
                var spawners = RecoverEntities(new List<Spawner>(), reader);
                _type2Entities[typeof(Enemy)] = enemies;
                _type2Entities[typeof(Player)] = players;
                _type2Entities[typeof(Spawner)] = spawners;
                //TODO
                //0. Recover Entities
                //1. Rebind Ref
                //2. Rebind Views 
                //    1. Find diff
                //    2. Pool Views
                //3. Recover Services
            }
            else {
                Debug.LogError($"Miss backup data  cannot rollback! {tick}");
            }
        }

        public override void Clean(int maxVerifiedTick){
            base.Clean(maxVerifiedTick);
        }

        void BackUpEntities<T>(List<T> lst, Serializer writer) where T : IBackup, new(){
            writer.Write(lst.Count);
            foreach (var item in lst) {
                item.WriteBackup(writer);
            }
        }

        List<T> RecoverEntities<T>(List<T> lst, Deserializer reader) where T : IBackup, new(){
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++) {
                var t = new T();
                lst.Add(t);
                t.ReadBackup(reader);
                if (t is IAfterBackup tt) {
                    tt.OnAfterDeserialize();
                }
            }

            return lst;
        }

        protected override FuncUndoCommands GetRollbackFunc(){
            return (minTickNode, maxTickNode, param) => { minTickNode.cmd.Undo(param); };
        }

        public struct GameState {
            public LFloat RemainTime;
            public LFloat DeltaTime;
            public int MaxEnemyCount;
            public int CurEnemyCount;
            public int CurEnemyId;
        }

        public LFloat RemainTime {
            get => _curGameState.RemainTime;
            set => _curGameState.RemainTime = value;
        }

        public LFloat DeltaTime {
            get => _curGameState.DeltaTime;
            set => _curGameState.DeltaTime = value;
        }
        public int MaxEnemyCount {
            get => _curGameState.MaxEnemyCount;
            set => _curGameState.MaxEnemyCount = value;
        }
        public int CurEnemyCount {
            get => _curGameState.CurEnemyCount;
            set => _curGameState.CurEnemyCount = value;
        }
        public int CurEnemyId {
            get => _curGameState.CurEnemyId;
            set => _curGameState.CurEnemyId = value;
        }
    }
}