using System;
using Lockstep.Math;

namespace Lockstep.Logic {
    [Serializable]
    public class BaseLifeCycle : ILifeCycle {
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }
}