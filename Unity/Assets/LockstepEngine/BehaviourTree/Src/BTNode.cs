using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lockstep.Logging;
using Lockstep.Serialization;

namespace Lockstep.BehaviourTree {
    public interface IBTNode{
    }

    public abstract unsafe partial class BTNode : BaseFormater ,IBTNode{
#if DEBUG
        [NoToBytes] public BTNode Parent;
        [NoToBytes] public BaseNodeData EditorInfo = new BaseNodeData();
#endif
        [NoToBytes]
        protected virtual int MemSize => 0;
        [NoToBytes]
        public virtual Type DataType => null;
        [NoToBytes]
        public int UniqueKey {
            get => _uniqueKey;
            set => _uniqueKey = value;
        }
        protected int _uniqueKey;

        private const int defaultChildCount = -1;
        private BTPrecondition _precondition;

        [NoToBytes]
        public BTPrecondition Precondition {
            get => _precondition;
            set {
#if DEBUG
                if (value != null) {
                    value.Parent = this;
                }
#endif
                _precondition = value;
            }
        }
        

        [NoToBytes]
        public List<BTNode> _children = new List<BTNode>();
        private int _maxChildCount;
        [NoToBytes]
        public int MaxChildNode => int.MaxValue;
        [NoToBytes]
        public virtual short NodeId =>
            (short) (int) (EBTBuildInTypeIdx) Enum.Parse(typeof(EBTBuildInTypeIdx), GetType().Name);
        public virtual object[] GetRuntimeData(BTWorkingData wData){
            return null;
        }
        public void InitNodeId(ref int curIdx){
            _uniqueKey = curIdx++;
            Precondition?.InitNodeId(ref curIdx);
            foreach (var node in _children) {
                node.InitNodeId(ref curIdx);
            }
        }

        public BTNode(int maxChildCount = -1){
            _maxChildCount = maxChildCount;
        }

        public BTNode()
            : this(defaultChildCount){ }

        ~BTNode(){
            _children = null;
        }

        public override void Serialize(Serializer writer){ }

        public override void Deserialize(Deserializer reader){}
        
        public int GetChildCount(){
            return _children.Count;
        }

        public BTNode GetChild(int idx){
            return _children[idx];
        }

        public List<BTNode> GetChildren(){
            return _children;
        }

        public virtual bool IsCanAddNode(){
            return _children.Count < _maxChildCount;
        }

        public BTNode GetClone(){
            return (BTNode) Activator.CreateInstance(GetType());
        }

        public BTNode DeepClone(){
            return (BTNode) Activator.CreateInstance(GetType());
        }

        public bool IsParentNode(BTNode node){
#if DEBUG
            if (node == null || Parent == null) return false;
            if (node == Parent) return true;
            if (Parent.IsParentNode(node)) return true;
#endif
            return false;
        }

        public bool IsIndexValid(int index){
            return index >= 0 && index < _children.Count;
        }


        public BTNode AddChild(BTNode node){
            if (_maxChildCount >= 0 && _children.Count >= _maxChildCount) {
                Logging.Debug.LogError("**BT** exceeding child count");
                return this;
            }

            _children.Add(node);
#if DEBUG
            node.Parent = this;
#endif
            return this;
        }

        public void RemoveChild(BTNode node){
            _children.Remove(node);
        }

        public void InsertChild(BTNode PreNode, BTNode node, bool back = false){
#if DEBUG
            if (IsParentNode(node)) {
                Debug.LogError("Can not make parent as it's child ");
                return;
            }
#endif
            int index = 0;
            int count = _children.Count;
            for (int i = 0; i < count; i++) {
                if (_children[i] == PreNode) {
                    index = i;
                    break;
                }
            }

            if (back) index += 1;
            _children.Insert(index, node);
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="prenode"></param>
        /// <param name="node"></param>
        public void ReplaceChild(BTNode PreNode, BTNode node){
#if DEBUG
            if (IsParentNode(node)) {
                Debug.LogError("Can not make parent as it's child ");
                return;
            }
#endif
            int index = -1;
            int count = _children.Count;
            for (int i = 0; i < count; i++) {
                if (_children[i] == PreNode) {
                    index = i;
                    break;
                }
            }

            if ((index < 0) || (index >= _children.Count)) {
                return;
            }

            _children[index] = node;
        }


        public T GetChild<T>(int index) where T : BTNode{
            if (index < 0 || index >= _children.Count) {
                return null;
            }

            return (T) _children[index];
        }


        public int GetTotalNodeCount(){
            int sum = 0;
            foreach (var child in _children) {
                sum += child.GetTotalNodeCount();
            }

            return sum + 1;
        }

        public int GetTotalMemSize(){
            int sum = 0;
            foreach (var child in _children) {
                sum += child.GetTotalMemSize();
            }

            return sum + MemSize;
        }

        public int[] GetTotalOffsets(){
            var nodes = new List<BTNode>();
            Flatten(nodes);
            var offsets = new int[nodes.Count];

            for (int i = 0; i < nodes.Count; i++) {
                Debug.Assert(nodes[i]._uniqueKey == i, "Error: Idx not match");
            }

            var offset = 0;
            for (int i = 0; i < nodes.Count; i++) {
                offsets[i] = offset;
                offset += nodes[i].MemSize;
            }

            return offsets;
        }

        protected virtual void Flatten(List<BTNode> nodes){
            nodes.Add(this);
            Precondition?.Flatten(nodes);
            foreach (var child in _children) {
                child.Flatten(nodes);
            }
        }

        // for unity debug
        public void OnSceneGUI(){ }
        public void ShowInspector(){ }
    }
}