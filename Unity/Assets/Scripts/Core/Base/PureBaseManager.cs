using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    public abstract class PureBaseManager : IManager {
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }
}