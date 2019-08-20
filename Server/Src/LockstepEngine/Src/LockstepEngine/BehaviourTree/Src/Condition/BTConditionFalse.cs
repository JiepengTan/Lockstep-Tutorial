namespace Lockstep.BehaviourTree {
    [BuildInNode(typeof(BTConditionFalse),EBTBuildInTypeIdx.BTConditionFalse)]
    public partial class BTConditionFalse : BTCondition {
        public override bool IsTrue( /*in*/ BTWorkingData wData){
            return false;
        }
    }
}