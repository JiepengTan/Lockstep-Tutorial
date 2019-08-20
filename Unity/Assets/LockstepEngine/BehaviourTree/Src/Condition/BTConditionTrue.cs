namespace Lockstep.BehaviourTree {
    [BuildInNode(typeof(BTConditionTrue),EBTBuildInTypeIdx.BTConditionTrue)]
    public partial class  BTConditionTrue : BTCondition {
        public override bool IsTrue( /*in*/ BTWorkingData wData){
            return true;
        }
    }
}