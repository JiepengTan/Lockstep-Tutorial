namespace Lockstep.BehaviourTree {
    public partial class  BTComposite : BTAction {
        public BTComposite(int maxChildCount)
            : base(maxChildCount){ }
        public BTComposite( )
            : this(int.MaxValue){ }
    }
}