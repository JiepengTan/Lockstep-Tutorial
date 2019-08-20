namespace Lockstep.BehaviourTree {
    
    public abstract partial class BTPreconditionUnary : BTDecorator {
        public BTPreconditionUnary()
            : base(1){ }

        public BTPreconditionUnary(BTPrecondition lhs)
            : base(1){
            AddChild(lhs);
        }
    }
    public abstract partial class BTDecorator : BTPrecondition {
        public BTDecorator(int maxChildCount) : base(maxChildCount){ }

    }

    public abstract partial class BTPreconditionBinary : BTDecorator {
        public BTPreconditionBinary()
            : base(2){ }

        public BTPreconditionBinary(BTPrecondition lhs, BTPrecondition rhs)
            : base(2){
            AddChild(lhs).AddChild(rhs);
        }
    }


    //---------------------------------------------------------------
    //unary precondition
    [BuildInNode(typeof(BTPreconditionNot),EBTBuildInTypeIdx.BTPreconditionNot)]
    public partial class BTPreconditionNot : BTPreconditionUnary {
        public BTPreconditionNot(){ }

        public BTPreconditionNot(BTPrecondition lhs, int uniqueKey)
            : base(lhs){
            _uniqueKey = uniqueKey;
        }

        public override bool IsTrue( /*in*/ BTWorkingData wData){
            return !GetChild<BTPrecondition>(0).IsTrue(wData);
        }
    }

    //---------------------------------------------------------------
    //binary precondition
    [BuildInNode(typeof(BTPreconditionAnd),EBTBuildInTypeIdx.BTPreconditionAnd)]
    public partial class BTPreconditionAnd : BTPreconditionBinary {
        public BTPreconditionAnd(){ }

        public BTPreconditionAnd(BTPrecondition lhs, BTPrecondition rhs, int uniqueKey)
            : base(lhs, rhs){
            _uniqueKey = uniqueKey;
        }

        public override bool IsTrue( /*in*/ BTWorkingData wData){
            return GetChild<BTPrecondition>(0).IsTrue(wData) &&
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }

    [BuildInNode(typeof(BTPreconditionOr),EBTBuildInTypeIdx.BTPreconditionOr)]
    public partial class BTPreconditionOr : BTPreconditionBinary {
        public BTPreconditionOr(){ }

        public BTPreconditionOr(BTPrecondition lhs, BTPrecondition rhs, int uniqueKey)
            : base(lhs, rhs){
            _uniqueKey = uniqueKey;
        }

        public override bool IsTrue( /*in*/ BTWorkingData wData){
            return GetChild<BTPrecondition>(0).IsTrue(wData) ||
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }

    [BuildInNode(typeof(BTPreconditionXor),EBTBuildInTypeIdx.BTPreconditionXor)]
    public partial class BTPreconditionXor : BTPreconditionBinary {
        public BTPreconditionXor(){ }

        public BTPreconditionXor(BTPrecondition lhs, BTPrecondition rhs, int uniqueKey)
            : base(lhs, rhs){
            _uniqueKey = uniqueKey;
        }

        public override bool IsTrue( /*in*/ BTWorkingData wData){
            return GetChild<BTPrecondition>(0).IsTrue(wData) ^
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }
}