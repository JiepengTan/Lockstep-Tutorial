using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class PureGameViewService : BaseService, IGameViewService {
        public virtual void BindView(BaseEntity entity){ }

        public virtual void UnbindView(BaseEntity entity){ }
    }
}