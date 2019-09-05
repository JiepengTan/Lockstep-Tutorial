using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IGameViewService : IService {
        void BindView(BaseEntity entity,BaseEntity oldEntity = null);
        void UnbindView(BaseEntity entity);
    }
}