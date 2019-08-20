using System;
using System.Collections.Generic;

namespace Lockstep.BehaviourTree {
    [BuildInNode(typeof(BTActionNonPrioritizedSelector), EBTBuildInTypeIdx.BTActionNonPrioritizedSelector)]
    public unsafe partial class BTActionNonPrioritizedSelector : BTActionPrioritizedSelector {
        public override object[] GetRuntimeData(BTWorkingData wData){
            return new object[] {
                *(BTCActionNonPrioritizedSelector*) wData.GetContext(_uniqueKey),
            };
        }

        protected override int MemSize => sizeof(BTCActionNonPrioritizedSelector);
        public override Type DataType => typeof(BTCActionNonPrioritizedSelector);

    

        protected override bool OnEvaluate( /*in*/ BTWorkingData wData){
            var thisContext = (BTCActionNonPrioritizedSelector*) wData.GetContext(_uniqueKey);
            //check last node first
            if (IsIndexValid(thisContext->currentSelectedIndex)) {
                BTAction node = GetChild<BTAction>(thisContext->currentSelectedIndex);
                if (node.Evaluate(wData)) {
                    return true;
                }
            }

            return base.OnEvaluate(wData);
        }
    }
}