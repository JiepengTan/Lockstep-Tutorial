using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;

namespace Lockstep.Game {
    public interface IInputService : IService {
        void Execute(InputCmd cmd, object entity);
        List<InputCmd> GetInputCmds();
        List<InputCmd> GetDebugInputCmds();
    }
}