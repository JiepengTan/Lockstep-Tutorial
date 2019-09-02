using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class EnemySystem : BaseSystem {
        private List<Spawner> Spawners => _gameStateService.GetSpawners();
        private List<Enemy> AllEnemy => _gameStateService.GetEnemies();


        public override void DoStart(){
            var spawnerInfos = _gameConfigService.SpawnerConfig.spawners;
            Spawners.Clear();
            foreach (var item in spawnerInfos) {
                Spawners.Add(new Spawner() {info = item});
            }
            foreach (var spawner in Spawners) {
                spawner.ServiceContainer = _serviceContainer;
                spawner.GameStateService = _gameStateService;
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

    }
}