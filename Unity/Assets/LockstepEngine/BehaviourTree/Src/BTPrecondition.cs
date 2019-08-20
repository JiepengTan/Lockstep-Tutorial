using System;

namespace Lockstep.BehaviourTree {
    //---------------------------------------------------------------
    public abstract unsafe partial class BTPrecondition : BTNode {
        public BTPrecondition(int maxChildCount)
            : base(maxChildCount){ }

        public abstract bool IsTrue( /*in*/ BTWorkingData wData);
    }
}