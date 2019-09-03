using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
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

        public AnimatorConfig GetAnimatorConfig(int id){
            return _config.GetAnimatorConfig(id);
        }

        public SkillBoxConfig GetSkillConfig(int id){
            return _config.GetSkillConfig(id);
        }


        public CollisionConfig CollisionConfig => _config.CollisionConfig;
        public SpawnerConfig SpawnerConfig => _config.SpawnerConfig;
        public string RecorderFilePath => _config.RecorderFilePath;
        public Msg_G2C_GameStartInfo ClientModeInfo => _config.ClientModeInfo;
    }
}