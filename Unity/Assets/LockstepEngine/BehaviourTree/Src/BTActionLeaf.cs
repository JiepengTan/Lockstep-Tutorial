using System;
using System.Collections.Generic;

namespace Lockstep.BehaviourTree {
    public abstract unsafe partial class BTActionLeaf : BTAction {
        private const int ACTION_READY = 0;
        private const int ACTION_RUNNING = 1;
        private const int ACTION_FINISHED = 2;

        public override object[] GetRuntimeData(BTWorkingData wData){
            return new object[] {*(BTCActionLeaf*) wData.GetContext(_uniqueKey)};
        }

        protected override int MemSize => sizeof(BTCActionLeaf);
        public override Type DataType => typeof(BTCActionLeaf);

        public BTActionLeaf()
            : base(0){ }

        protected sealed override int OnUpdate(BTWorkingData wData){
            int runningState = BTRunningStatus.FINISHED;
            var thisContext = (BTCActionLeaf*) wData.GetContext(_uniqueKey);
#if DEBUG
            BTCActionLeaf __DEBUGval = *thisContext;
#endif
            if (thisContext->status == ACTION_READY) {
                OnEnter(wData);
                thisContext->needExit = true;
                thisContext->status = ACTION_RUNNING;
            }

            if (thisContext->status == ACTION_RUNNING) {
                runningState = OnExecute(wData);
                if (BTRunningStatus.IsFinished(runningState)) {
                    thisContext->status = ACTION_FINISHED;
                }
            }

            if (thisContext->status == ACTION_FINISHED) {
                if (thisContext->needExit) {
                    OnExit(wData, runningState);
                }

                thisContext->status = ACTION_READY;
                thisContext->needExit = false;
            }

            return runningState;
        }

        protected sealed override void OnTransition(BTWorkingData wData){
            var thisContext = (BTCActionLeaf*) wData.GetContext(_uniqueKey);
            if (thisContext->needExit) {
                OnExit(wData, BTRunningStatus.TRANSITION);
            }

            thisContext->status = ACTION_READY;
            thisContext->needExit = false;
        }

        protected void* GetUserContextData(BTWorkingData wData){
            return ((byte*) wData.GetContext(_uniqueKey) + sizeof(BTCActionLeaf));
        }

        //--------------------------------------------------------
        // inherented by children-
        protected virtual void OnEnter( /*in*/ BTWorkingData wData){ }

        protected virtual int OnExecute(BTWorkingData wData){
            return BTRunningStatus.FINISHED;
        }

        protected virtual void OnExit(BTWorkingData wData, int runningStatus){ }
    }
}