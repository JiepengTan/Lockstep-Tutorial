using System;

namespace Lockstep.BehaviourTree {
    public unsafe partial class BTRunningStatus {
        //-------------------------------------------------------
        //Any value which is below ZERO means error occurs 
        //-------------------------------------------------------
        //default running status
        public const int EXECUTING = 0;
        public const int FINISHED = 1;

        public const int TRANSITION = 2;

        //-------------------------------------------------------
        //User running status
        //50-100, reserved user executing status
        public const int USER_EXECUTING = 50;

        //>=100, reserved user finished status
        public const int USER_FINISHED = 100;

        //-------------------------------------------------------
        public static bool IsOK(int runningStatus){
            return runningStatus == BTRunningStatus.FINISHED ||
                   runningStatus >= BTRunningStatus.USER_FINISHED;
        }

        public static bool IsError(int runningStatus){
            return runningStatus < 0;
        }

        public static bool IsFinished(int runningStatus){
            return IsOK(runningStatus) || IsError(runningStatus);
        }

        public static bool IsExecuting(int runningStatus){
            return !IsFinished(runningStatus);
        }
    }
}