using System;
using System.Collections.Generic;

namespace Lockstep.BehaviourTree {
    [BuildInNode(typeof(BTActionPrioritizedSelector),EBTBuildInTypeIdx.BTActionPrioritizedSelector)]
    public unsafe partial class BTActionPrioritizedSelector : BTComposite {      
        public override object[] GetRuntimeData(BTWorkingData wData){
            return new object[] {
                *(BTCActionPrioritizedSelector*) wData.GetContext(_uniqueKey),
            };
        }
        public override Type DataType => typeof(BTCActionPrioritizedSelector);
        protected override int MemSize => sizeof(BTCActionPrioritizedSelector);


        protected override bool OnEvaluate( /*in*/ BTWorkingData wData){
            var thisContext = (BTCActionPrioritizedSelector*) wData.GetContext(_uniqueKey);
            thisContext->currentSelectedIndex = -1;
            int childCount = GetChildCount();
            for (int i = 0; i < childCount; ++i) {
                BTAction node = GetChild<BTAction>(i);
                if (node.Evaluate(wData)) {
                    thisContext->currentSelectedIndex = i;
                    return true;
                }
            }

            return false;
        }

        protected override int OnUpdate(BTWorkingData wData){
            var thisContext = (BTCActionPrioritizedSelector*) wData.GetContext(_uniqueKey);
            int runningState = BTRunningStatus.FINISHED;
            if (thisContext->currentSelectedIndex != thisContext->lastSelectedIndex) {
                if (IsIndexValid(thisContext->lastSelectedIndex)) {
                    BTAction node = GetChild<BTAction>(thisContext->lastSelectedIndex);
                    node.Transition(wData);
                }

                thisContext->lastSelectedIndex = thisContext->currentSelectedIndex;
            }

            if (IsIndexValid(thisContext->lastSelectedIndex)) {
                BTAction node = GetChild<BTAction>(thisContext->lastSelectedIndex);
                runningState = node.Update(wData);
                if (BTRunningStatus.IsFinished(runningState)) {
                    thisContext->lastSelectedIndex = -1;
                }
            }

            return runningState;
        }

        protected override void OnTransition(BTWorkingData wData){
            var thisContext = (BTCActionPrioritizedSelector*) wData.GetContext(_uniqueKey);
            BTAction node = GetChild<BTAction>(thisContext->lastSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }

            thisContext->lastSelectedIndex = -1;
        }
    }
}