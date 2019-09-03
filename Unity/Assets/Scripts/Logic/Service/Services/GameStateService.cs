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
        private Dictionary<int, BaseEntity> _id2Entities = new Dictionary<int, BaseEntity>();
        private Dictionary<int, Serializer> _tick2Backup = new Dictionary<int, Serializer>();

        private void AddEntity<T>(T e) where T : BaseEntity{
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

            _id2Entities[e.EntityId] = e;
        }

        private void RemoveEntity<T>(T e) where T : BaseEntity{
            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj)) {
                var lst = lstObj as List<T>;
                lst.Remove(e);
                _id2Entities.Remove(e.EntityId);
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

        public T CreateEntity<T>(int prefabId, LVector3 position) where T : BaseEntity, new(){
            Debug.Trace($"CreateEntity {prefabId} pos {prefabId}");
            var config = _gameConfigService.GetEnemyConfig(prefabId);
            var baseEntity = new T();
            config.CopyTo(baseEntity); 
            baseEntity.EntityId = _idService.GenId();
            baseEntity.PrefabId = prefabId;
            baseEntity.GameStateService = _gameStateService;
            baseEntity.ServiceContainer = _serviceContainer;
            baseEntity.transform.Pos3 = position;
            baseEntity.DoBindRef();
            if (baseEntity is Entity entity) {
                PhysicSystem.Instance.RegisterEntity(prefabId, entity);
            }

            baseEntity.DoAwake();
            baseEntity.DoStart();
            _gameViewService.BindView(baseEntity);
            AddEntity(baseEntity);
            return baseEntity;
        }

        public void DestroyEntity(BaseEntity entity){
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
                //.TODO reduce the unnecessary create and destroy 
                var reader = new Deserializer(backupData.Data);
                //. Unbind Entity views
                foreach (var pair in _id2Entities) {
                    _gameViewService.UnbindView(pair.Value);
                }

                _id2Entities.Clear();
                _type2Entities.Clear();

                //. Recover Entities
                RecoverEntities(new List<Enemy>(), reader);
                RecoverEntities(new List<Player>(), reader);
                RecoverEntities(new List<Spawner>(), reader);

                //. Rebind Ref
                foreach (var entity in _id2Entities.Values) {
                    entity.GameStateService = _gameStateService;
                    entity.ServiceContainer = _serviceContainer;
                    entity.DoBindRef();
                }

                //. Rebind Views 
                foreach (var pair in _id2Entities) {
                    _gameViewService.BindView(pair.Value);
                }
            }
            else {
                Debug.LogError($"Miss backup data  cannot rollback! {tick}");
            }
        }


        public override void Clean(int maxVerifiedTick){
            base.Clean(maxVerifiedTick);
        }

        void BackUpEntities<T>(List<T> lst, Serializer writer) where T : BaseEntity, IBackup, new(){
            writer.Write(lst.Count);
            foreach (var item in lst) {
                item.WriteBackup(writer);
            }
        }

        List<T> RecoverEntities<T>(List<T> lst, Deserializer reader) where T : BaseEntity, IBackup, new(){
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++) {
                var t = new T();
                lst.Add(t);
                t.ReadBackup(reader);
            }

            _type2Entities[typeof(T)] = lst;
            foreach (var e in lst) {
                _id2Entities[e.EntityId] = e;
            }

            return lst;
        }


        public class CopyStateCmd : BaseCommand {
            private GameState _state;

            public override void Do(object param){
                _state = ((GameStateService) param)._curGameState;
            }

            public override void Undo(object param){
                ((GameStateService) param)._curGameState = _state;
            }
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