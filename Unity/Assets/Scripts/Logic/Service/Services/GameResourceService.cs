using System;
using System.Collections.Generic;
using System.Reflection;
using Lockstep.Game;
using UnityEngine;

namespace Lockstep.Game {
    public class GameResourceService : BaseGameService, IGameResourceService {
        public static GameResourceService Instance { get; private set; }
        public string pathPrefix = "Prefabs/";
        
        private Dictionary<int, GameObject> _id2Prefab = new Dictionary<int, GameObject>();

        public override void DoAwake(IServiceContainer container){
            Instance = this;
        }

        public static AnimatorConfig LoadAnimConfig(int id){
            return null;
        }
        public object LoadPrefab(int id){
            return _LoadPrefab(id);
        }


        GameObject _LoadPrefab(int id){
            if (_id2Prefab.TryGetValue(id, out var val)) {
                return val;
            }

            if (id < 10) {
                var config = _gameConfigService.GetPlayerConfig(id);
                var prefab = (GameObject) Resources.Load(pathPrefix + config.prefabPath);
                _id2Prefab[id] = prefab;
                PhysicSystem.Instance.RigisterPrefab(prefab, (int) EColliderLayer.Hero);
                return prefab;
            }

            if (id >= 10) {
                var config = _gameConfigService.GetEnemyConfig(id );
                var prefab = (GameObject) Resources.Load(pathPrefix + config.prefabPath);
                _id2Prefab[id] = prefab;
                PhysicSystem.Instance.RigisterPrefab(prefab, (int) EColliderLayer.Enemy);
                return prefab;
            }

            return null;
        }
    }
}