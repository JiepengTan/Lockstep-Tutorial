using System;
using System.Collections.Generic;

namespace Lockstep.BehaviourTree {
    [BuildInNode(typeof(BTActionParallel),EBTBuildInTypeIdx.BTActionParallel)]
    public unsafe partial class BTActionParallel : BTComposite {
        public enum ECHILDREN_RELATIONSHIP {
            AND,
            OR
        }
        public override object[] GetRuntimeData(BTWorkingData wData){
            return new object[] {
                *(BTCActionParallel*) wData.GetContext(_uniqueKey),
            };
        }
        protected override int MemSize => sizeof(BTCActionParallel);
        public override Type DataType => typeof(BTCActionParallel);
        //-------------------------------------------------------
        private ECHILDREN_RELATIONSHIP _evaluationRelationship;

        private ECHILDREN_RELATIONSHIP _runningStatusRelationship;

        //-------------------------------------------------------
        public BTActionParallel()
            : base(){
            _evaluationRelationship = ECHILDREN_RELATIONSHIP.AND;
            _runningStatusRelationship = ECHILDREN_RELATIONSHIP.OR;
        }

        public BTActionParallel SetEvaluationRelationship(ECHILDREN_RELATIONSHIP v){
            _evaluationRelationship = v;
            return this;
        }

        public BTActionParallel SetRunningStatusRelationship(ECHILDREN_RELATIONSHIP v){
            _runningStatusRelationship = v;
            return this;
        }

        //------------------------------------------------------
        protected override bool OnEvaluate( /*in*/ BTWorkingData wData){
            var thisContext = (BTCActionParallel*) wData.GetContext(_uniqueKey);
            thisContext->evaluationStatus.Init(false);
            bool finalResult = false;
            for (int i = 0; i < GetChildCount(); ++i) {
                BTAction node = GetChild<BTAction>(i);
                bool ret = node.Evaluate(wData);
                //early break
                if (_evaluationRelationship == ECHILDREN_RELATIONSHIP.AND && ret == false) {
                    finalResult = false;
                    break;
                }

                if (ret == true) {
                    finalResult = true;
                }

                thisContext->evaluationStatus[i] = ret;
            }

            return finalResult;
        }

        protected override int OnUpdate(BTWorkingData wData){
            var thisContext = (BTCActionParallel*) wData.GetContext(_uniqueKey);
            //first time initialization

            bool hasFinished = false;
            bool hasExecuting = false;
            for (int i = 0; i < GetChildCount(); ++i) {
                if (thisContext->evaluationStatus[i] == false) {
                    continue;
                }

                if (BTRunningStatus.IsFinished(thisContext->StatusRunning[i])) {
                    hasFinished = true;
                    continue;
                }

                BTAction node = GetChild<BTAction>(i);
                int runningStatus = node.Update(wData);
                if (BTRunningStatus.IsFinished(runningStatus)) {
                    hasFinished = true;
                }
                else {
                    hasExecuting = true;
                }

                thisContext->StatusRunning[i] = (byte) runningStatus;
            }

            if (_runningStatusRelationship == ECHILDREN_RELATIONSHIP.OR && hasFinished ||
                _runningStatusRelationship == ECHILDREN_RELATIONSHIP.AND && hasExecuting == false) {
                thisContext->StatusRunning.Init((byte) (int) BTRunningStatus.EXECUTING);
                return BTRunningStatus.FINISHED;
            }

            return BTRunningStatus.EXECUTING;
        }

        protected override void OnTransition(BTWorkingData wData){
            var thisContext = (BTCActionParallel*) wData.GetContext(_uniqueKey);
            for (int i = 0; i < GetChildCount(); ++i) {
                BTAction node = GetChild<BTAction>(i);
                node.Transition(wData);
            }

            //clear running status
            thisContext->StatusRunning.Init((byte) (int) BTRunningStatus.EXECUTING);
        }
    }
}