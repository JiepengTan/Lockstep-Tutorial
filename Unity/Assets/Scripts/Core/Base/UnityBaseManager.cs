using System;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Logic {
    [Serializable]
    public abstract class UnityBaseManager : MonoBehaviour, IManager {
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoUpdate(LFloat deltaTime){ }
        public virtual void DoDestroy(){ }
    }
}