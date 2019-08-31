using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class GameConfigService : BaseGameService, IGameConfigService {
        private GameConfig _config;
        public string configPath = "GameConfig";
        public override void DoAwake(IServiceContainer container){
            _config = Resources.Load<GameConfig>(configPath);
        }
        public EnemyConfig GetEnemyConfig(int id){
            return _config.GetEnemyConfig(id - 10);
        }

        public PlayerConfig GetPlayerConfig(int id){
            return _config.GetPlayerConfig(id);
        }

        public CollisionConfig CollisionConfig => _config.CollisionConfig;
        public SpawnerConfig SpawnerConfig=> _config.SpawnerConfig;
        public string RecorderFilePath=> _config.RecorderFilePath;

        
        
    }
}