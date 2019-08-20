namespace Lockstep.BehaviourTree {
    public enum EBTBuildInTypeIdx{       
        BTConditionFalse                   = 0 ,     
        BTConditionTrue                    = 1 ,     
        BTPreconditionNot                  = 2 ,     
        BTPreconditionAnd                  = 3 ,     
        BTPreconditionOr                   = 4 ,     
        BTPreconditionXor                  = 5 ,
        BTActionLoop                       = 6 ,     
        BTActionNonPrioritizedSelector     = 7 ,     
        BTActionParallel                   = 8 ,     
        BTActionPrioritizedSelector        = 9 ,     
        BTActionSequence                   = 10,    
        MaxEnumCount = 100
    }
}