using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class GameEntityService : BaseGameService, IGameEntityService {
        public int enmeyID;

        public Enemy CreateEnemy(int prefabId, LVector3 position){
            var prefab = _gameResourceService.LoadPrefab(prefabId);
            var config = _gameConfigService.GetEnemyConfig(prefabId);
            Debug.Trace("CreateEnemy");
            var entity = new Enemy();
            var obj = CreateEntity(entity, prefabId, position, prefab, config);
#if UNITY_EDITOR
            obj.name = obj.name + enmeyID++;
#endif
            return entity;
        }

        public void CreatePlayer(Player entity, int prefabId, LVector3 position){
            var prefab = _gameResourceService.LoadPrefab(prefabId);
            var config = _gameConfigService.GetPlayerConfig(prefabId);
            CreateEntity(entity, prefabId, position, prefab, config);
        }

        private GameObject CreateEntity(Entity entity, int prefabId, LVector3 position, object prefabObj,
            EntityConfig config){
            var prefab = prefabObj as GameObject;
            var obj = (GameObject) GameObject.Instantiate(prefab, position.ToVector3(), Quaternion.identity);
            entity.GameStateService = _gameStateService;
            entity.engineTransform = obj.transform;
            entity.transform.Pos3 = position;
            config.CopyTo(entity);
            entity.PrefabId = prefabId;
            PhysicSystem.Instance.RegisterEntity(prefab, obj, entity);
            entity.DoAwake();
            entity.DoStart();
            var views = obj.GetComponents<IView>();
            foreach (var view in views) {
                view.BindEntity(entity);
            }

            return obj;
        }
    }
}