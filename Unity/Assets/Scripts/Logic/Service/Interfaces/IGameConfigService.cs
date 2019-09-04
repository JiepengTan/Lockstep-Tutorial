using System.Collections.Generic;
using Lockstep.ECS;
using Lockstep.Math;
using Lockstep.Game;
using NetMsg.Common;

namespace Lockstep.Game {
    public interface IGameConfigService : IService {
        EntityConfig GetEntityConfig(int id);
        AnimatorConfig GetAnimatorConfig(int id);
        SkillBoxConfig GetSkillConfig(int id);

        CollisionConfig CollisionConfig { get; }
        string RecorderFilePath { get; }
        string DumpStrPath { get; }
        Msg_G2C_GameStartInfo ClientModeInfo{ get; }
    }
}