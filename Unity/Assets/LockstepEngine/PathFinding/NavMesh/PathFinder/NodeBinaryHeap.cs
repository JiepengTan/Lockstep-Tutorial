using System;
using System.Text;
using Lockstep.Math;

namespace Lockstep.PathFinding {
/** @author Nathan Sweet */
    public class Node {
        public LFloat value; //节点排序比较值
        public int index; //节点索引

        public Node(LFloat value){
            this.value = value;
        }

        public LFloat GetValue(){
            return value;
        }

        public override String ToString(){
            return value.ToString();
        }
    }

/**
 * 二叉堆
 *  @author Nathan Sweet */
    public class NodeBinaryHeap<T> where T : Node {
        public int size;

        private Node[] _nodes;
        private bool _isMaxHeap;

        public NodeBinaryHeap() : this(16, false){ }

        public NodeBinaryHeap(int capacity, bool isMaxHeap){
            this._isMaxHeap = isMaxHeap;
            _nodes = new Node[capacity];
        }

        public T Add(T node){
            // Expand if necessary.
            if (size == _nodes.Length) {
                Node[] newNodes = new Node[size << 1];
                Array.Copy(_nodes, 0, newNodes, 0, size);
                _nodes = newNodes;
            }

            // Insert at end and bubble up.
            node.index = size;
            _nodes[size] = node;
            Up(size++);
            return node;
        }

        public T Add(T node, LFloat value){
            node.value = value;
            return Add(node);
        }

        public T Peek(){
            if (size == 0)
                throw new Exception("The heap is empty.");
            return (T) _nodes[0];
        }

        public T Pop(){
            return Remove(0);
        }

        public T Remove(T node){
            return Remove(node.index);
        }

        private T Remove(int index){
            Node[] nodes = this._nodes;
            Node removed = nodes[index];
            nodes[index] = nodes[--size];
            nodes[size] = null;
            if (size > 0 && index < size)
                Down(index);
            return (T) removed;
        }

        //TODO 可以不用对nodes 进行清零
        public void Clear(){
            Node[] nodes = this._nodes;
            for (int i = 0, n = size; i < n; i++)
                nodes[i] = null;
            size = 0;
        }

        public void SetValue(T node, LFloat value){
            LFloat oldValue = node.value;
            node.value = value;
            if ((value < oldValue) ^ _isMaxHeap)
                Up(node.index);
            else
                Down(node.index);
        }

        private void Up(int index){
            Node[] nodes = this._nodes;
            Node node = nodes[index];
            LFloat value = node.value;
            while (index > 0) {
                int parentIndex = (index - 1) >> 1;
                Node parent = nodes[parentIndex];
                if ((value < parent.value) ^ _isMaxHeap) {
                    nodes[index] = parent;
                    parent.index = index;
                    index = parentIndex;
                }
                else
                    break;
            }

            nodes[index] = node;
            node.index = index;
        }

        public static readonly LFloat MinValue = new LFloat(true, int.MinValue);

        private void Down(int index){
            Node[] nodes = this._nodes;
            int size = this.size;

            Node node = nodes[index];
            LFloat value = node.value;

            while (true) {
                int leftIndex = 1 + (index << 1);
                if (leftIndex >= size)
                    break;

                // Always have a left child.
                Node leftNode = nodes[leftIndex];
                LFloat leftValue = leftNode.value;

                // May have a right child.
                int rightIndex = leftIndex + 1;
                Node rightNode;
                LFloat rightValue;
                if (rightIndex >= size) {
                    rightNode = null;
                    rightValue = _isMaxHeap ? LFloat.MinValue : LFloat.MaxValue;
                }
                else {
                    rightNode = nodes[rightIndex];
                    rightValue = rightNode.value;
                }

                // The smallest of the three values is the parent.
                if ((leftValue < rightValue) ^ _isMaxHeap) {
                    if (leftValue == value || ((leftValue > value) ^ _isMaxHeap))
                        break;
                    nodes[index] = leftNode;
                    leftNode.index = index;
                    index = leftIndex;
                }
                else {
                    if (rightValue == value || ((rightValue > value) ^ _isMaxHeap))
                        break;
                    nodes[index] = rightNode;
                    rightNode.index = index;
                    index = rightIndex;
                }
            }

            nodes[index] = node;
            node.index = index;
        }


        public override int GetHashCode(){
            int h = 1;
            for (int i = 0, n = size; i < n; i++)
                h = h * 31 + _nodes[i].value._val;
            return h;
        }


        public override String ToString(){
            if (size == 0)
                return "[]";
            Node[] nodes = this._nodes;
            StringBuilder buffer = new StringBuilder(32);
            buffer.Append('[');
            buffer.Append(nodes[0].value);
            for (int i = 1; i < size; i++) {
                buffer.Append(", ");
                buffer.Append(nodes[i].value);
            }

            buffer.Append(']');
            return buffer.ToString();
        }
    }
}