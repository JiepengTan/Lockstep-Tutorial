using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class PureGameViewService : BaseService, IGameViewService {
        public void BindView(BaseEntity entity, BaseEntity oldEntity = null){ }

        public virtual void UnbindView(BaseEntity entity){ }
    }
}