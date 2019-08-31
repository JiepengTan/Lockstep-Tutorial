using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class EnemySystem : BaseSystem {
        private List<Spawner> Spawners => _gameStateService.GetSpawners();
        private List<Enemy> AllEnemy => _gameStateService.GetEnemies();

        public static int maxCount = 1;
        private static int curCount = 0;
        private static int enmeyID = 0;

        public override void DoStart(){
            var spawnerInfos = _gameConfigService.SpawnerConfig.spawners;
            Spawners.Clear();
            foreach (var item in spawnerInfos) {
                Spawners.Add(new Spawner() {info = item});
            }
            foreach (var spawner in Spawners) {
                spawner.OnSpawnEvent += OnSpawn;
                spawner.DoStart();
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            foreach (var spawner in Spawners) {
                spawner.DoUpdate(deltaTime);
            }

            foreach (var enemy in AllEnemy) {
                enemy.DoUpdate(deltaTime);
            }
        }

        private void OnSpawn(int prefabId, LVector3 position){
#if UNITY_EDITOR
            if (curCount >= maxCount) {
                return;
            }

            curCount++;
#endif

            var entity = _gameEntityService.CreateEnemy(prefabId, position) as Enemy;
            entity.OnDied += (e) => { RemoveEnemy(e as Enemy); };
            AddEnemy(entity);
        }


        public void AddEnemy(Enemy enemy){
            AllEnemy.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy){
            AllEnemy.Remove(enemy);
        }
    }
}