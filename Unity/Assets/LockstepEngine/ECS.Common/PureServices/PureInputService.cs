using System.Collections.Generic;

using NetMsg.Common;

namespace Lockstep.Game {
    public class PureInputService : PureBaseService, IInputService {
        List<InputCmd> cmds = new List<InputCmd>();
        public void Execute(InputCmd cmd, object entity){ }
        public List<InputCmd> GetInputCmds(){
            return cmds;
        }
    }
}