using System;
using System.Collections.Generic;

namespace Lockstep.BehaviourTree {
    [BuildInNode(typeof(BTActionSequence), EBTBuildInTypeIdx.BTActionSequence)]
    public unsafe partial class BTActionSequence : BTComposite {
        public override object[] GetRuntimeData(BTWorkingData wData){
            return new object[] {
                *(BTCActionSequence*) wData.GetContext(_uniqueKey),
            };
        }
        protected override int MemSize => sizeof(BTCActionSequence);

        public override Type DataType => typeof(BTCActionSequence);

        //-------------------------------------------------------
        private bool _continueIfErrorOccors;

        //-------------------------------------------------------
        public BTActionSequence(){
            _continueIfErrorOccors = false;
        }

        public BTActionSequence SetContinueIfErrorOccors(bool v){
            _continueIfErrorOccors = v;
            return this;
        }

        //------------------------------------------------------
        protected override bool OnEvaluate( /*in*/ BTWorkingData wData){
            var thisContext = (BTCActionSequence*) wData.GetContext(_uniqueKey);
            int checkedNodeIndex = -1;
            if (IsIndexValid(thisContext->currentSelectedIndex)) {
                checkedNodeIndex = thisContext->currentSelectedIndex;
            }
            else {
                checkedNodeIndex = 0;
            }

            if (IsIndexValid(checkedNodeIndex)) {
                BTAction node = GetChild<BTAction>(checkedNodeIndex);
                if (node.Evaluate(wData)) {
                    thisContext->currentSelectedIndex = checkedNodeIndex;
                    return true;
                }
            }

            return false;
        }

        protected override int OnUpdate(BTWorkingData wData){
            var thisContext = (BTCActionSequence*) wData.GetContext(_uniqueKey);
            int runningStatus = BTRunningStatus.FINISHED;
            BTAction node = GetChild<BTAction>(thisContext->currentSelectedIndex);
            runningStatus = node.Update(wData);
            if (_continueIfErrorOccors == false && BTRunningStatus.IsError(runningStatus)) {
                thisContext->currentSelectedIndex = -1;
                return runningStatus;
            }

            if (BTRunningStatus.IsFinished(runningStatus)) {
                thisContext->currentSelectedIndex++;
                if (IsIndexValid(thisContext->currentSelectedIndex)) {
                    runningStatus = BTRunningStatus.EXECUTING;
                }
                else {
                    thisContext->currentSelectedIndex = -1;
                }
            }

            return runningStatus;
        }

        protected override void OnTransition(BTWorkingData wData){
            var thisContext = (BTCActionSequence*) wData.GetContext(_uniqueKey);
            BTAction node = GetChild<BTAction>(thisContext->currentSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }

            thisContext->currentSelectedIndex = -1;
        }
    }
}