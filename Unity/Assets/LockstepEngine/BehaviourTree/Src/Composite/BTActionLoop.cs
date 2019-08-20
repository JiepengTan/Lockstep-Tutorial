using System;
using System.Runtime.InteropServices;
using Lockstep.Util;

namespace Lockstep.BehaviourTree
{
    [BuildInNode(typeof(BTActionLoop),EBTBuildInTypeIdx.BTActionLoop)]
    public unsafe partial class BTActionLoop : BTComposite
    {
        public const int INFINITY = -1;
        [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
        public unsafe partial struct BTActionLoopContext 
        {
            internal int currentCount;
        }
        //--------------------------------------------------------
        private int _loopCount;
        //--------------------------------------------------------
        public BTActionLoop()
            : base(1)
        {
            _loopCount = INFINITY;
        }
        public BTActionLoop SetLoopCount(int count)
        {
            _loopCount = count;
            return this;
        }
        //-------------------------------------------------------
        protected override bool OnEvaluate(/*in*/BTWorkingData wData)
        {
            var thisContext = (BTActionLoopContext*)wData.GetContext(_uniqueKey);
            bool checkLoopCount = (_loopCount == INFINITY || thisContext->currentCount < _loopCount);
            if (checkLoopCount == false) {
                return false;
            }
            if (IsIndexValid(0)) {
                BTAction node = GetChild<BTAction>(0);
                return node.Evaluate(wData);
            }
            return false;
        }
        protected override int OnUpdate(BTWorkingData wData)
        {
            var thisContext = (BTActionLoopContext*)wData.GetContext(_uniqueKey);
            int runningStatus = BTRunningStatus.FINISHED;
            if (IsIndexValid(0)) {
                BTAction node = GetChild<BTAction>(0);
                runningStatus = node.Update(wData);
                if (BTRunningStatus.IsFinished(runningStatus)) {
                    thisContext->currentCount++;
                    if (thisContext->currentCount < _loopCount || _loopCount == INFINITY) {
                        runningStatus = BTRunningStatus.EXECUTING;
                    }
                }
            }
            return runningStatus;
        }
        protected override void OnTransition(BTWorkingData wData)
        {
            var thisContext = (BTActionLoopContext*)wData.GetContext(_uniqueKey);
            if (IsIndexValid(0)) {
                BTAction node = GetChild<BTAction>(0);
                node.Transition(wData);
            }
            thisContext->currentCount = 0;
        }
    }
}
