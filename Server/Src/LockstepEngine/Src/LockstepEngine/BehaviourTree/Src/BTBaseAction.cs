using System;
using System.Collections.Generic;
using Lockstep.Serialization;

namespace Lockstep.BehaviourTree {
    public class BTActionContext { }

    public abstract unsafe partial class BTAction : BTNode {
        
        //-------------------------------------------------------------
        public BTAction(int maxChildCount)
            : base(maxChildCount){
        }

        ~BTAction(){
            Precondition = null;
        }

        //-------------------------------------------------------------
        public bool Evaluate( /*in*/ BTWorkingData wData){
            return (Precondition == null || Precondition.IsTrue(wData)) && OnEvaluate(wData);
        }

        public int Update(BTWorkingData wData){
            wData.HasVisitedInfos[_uniqueKey] = true;
            return OnUpdate(wData);
        }

        public void Transition(BTWorkingData wData){
            OnTransition(wData);
        }

        public BTAction SetPrecondition(BTPrecondition precondition){
            Precondition = precondition;
            return this;
        }

        public override int GetHashCode(){
            return _uniqueKey;
        }

        //--------------------------------------------------------
        // inherented by children
        protected virtual bool OnEvaluate( /*in*/ BTWorkingData wData){
            return true;
        }

        protected virtual int OnUpdate(BTWorkingData wData){
            return BTRunningStatus.FINISHED;
        }

        protected virtual void OnTransition(BTWorkingData wData){ }
    }
}