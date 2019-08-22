using System;
using System.Collections.Generic;
using System.Reflection;
using Lockstep.Logic;
using UnityEngine;

namespace LockstepTutorial {
    public class ResourceManager : UnityBaseManager {
        public static ResourceManager Instance { get; private set; }
        [HideInInspector] public GameConfig config;
        public string configPath = "GameConfig";
        public string pathPrefix = "Prefabs/";
        
        private Dictionary<int, GameObject> _id2Prefab = new Dictionary<int, GameObject>();

        public override void DoAwake(){
            Instance = this;
            config = Resources.Load<GameConfig>(configPath);
        }

        public static AnimatorConfig LoadAnimConfig(int id){
            return null;
        }
        public static GameObject LoadPrefab(int id){
            return Instance._LoadPrefab(id);
        }

        public static PlayerConfig GetPlayerConfig(int id){
            return Instance.config.GetPlayerConfig(id);
        }

        public static EnemyConfig GetEnemyConfig(int id){
            return Instance.config.GetEnemyConfig(id - 10);
        }

        GameObject _LoadPrefab(int id){
            if (_id2Prefab.TryGetValue(id, out var val)) {
                return val;
            }

            if (id < 10) {
                var config = this.config.GetPlayerConfig(id);
                var prefab = (GameObject) Resources.Load(pathPrefix + config.prefabPath);
                _id2Prefab[id] = prefab;
                CollisionManager.Instance.RigisterPrefab(prefab, (int) EColliderLayer.Hero);
                return prefab;
            }

            if (id >= 10) {
                var config = this.config.GetEnemyConfig(id - 10);
                var prefab = (GameObject) Resources.Load(pathPrefix + config.prefabPath);
                _id2Prefab[id] = prefab;
                CollisionManager.Instance.RigisterPrefab(prefab, (int) EColliderLayer.Enemy);
                return prefab;
            }

            return null;
        }
    }
}