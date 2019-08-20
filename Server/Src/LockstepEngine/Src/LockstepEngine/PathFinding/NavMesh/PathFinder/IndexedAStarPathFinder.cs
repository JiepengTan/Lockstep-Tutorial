using System;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public class IndexedAStarPathFinder<N> : PathFinder<N> {
        private IndexedGraph<N> _graph;
        public NodeRecord<N>[] _nodeRecords;

        private NodeBinaryHeap<NodeRecord<N>> _openList;

        /** The unique ID for each search run. Used to mark nodes. */
        private int _searchId;
        private NodeRecord<N> _current;

        public Metrics metrics;

        public const int UNVISITED = 0;
        public const int OPEN = 1;
        public const int CLOSED = 2;

        public IndexedAStarPathFinder(IndexedGraph<N> graph) : this(graph, false){ }

        public IndexedAStarPathFinder(IndexedGraph<N> graph, bool calculateMetrics){
            this._graph = graph;
            this._nodeRecords = new NodeRecord<N>[graph.GetNodeCount()];
            this._openList = new NodeBinaryHeap<NodeRecord<N>>();
            if (calculateMetrics) this.metrics = new Metrics();
        }

        public bool SearchPath(N startNode, N endNode, Heuristic<N> heuristic, GraphPath<Connection<N>> outPath){
            if (startNode == null) {
                Debug.LogError("起点坐标不在寻路层中");
                return false;
            }

            if (endNode == null) {
                Debug.LogError("终点坐标不在寻路层中");
                return false;
            }

            // Perform AStar
            bool found = Search(startNode, endNode, heuristic);

            if (found) {
                // Create a path made of connections
                GeneratePath(startNode, outPath);
            }

            return found;
        }

        public bool SearchNodePath(N startNode, N endNode, Heuristic<N> heuristic, GraphPath<N> outPath){
            // Perform AStar
            bool found = Search(startNode, endNode, heuristic);

            if (found) {
                // Create a path made of nodes
                GenerateNodePath(startNode, outPath);
            }

            return found;
        }

        protected bool Search(N startNode, N endNode, Heuristic<N> heuristic){
            InitSearch(startNode, endNode, heuristic);

            // Iterate through processing each node
            do {
                // Retrieve the node with smallest estimated total cost from the open list
                _current = _openList.Pop();
                _current.category = CLOSED;

                // Terminate if we reached the goal node
                if (_current.node.Equals(endNode)) return true;

                VisitChildren(endNode, heuristic);
            } while (_openList.size > 0);

            // We've run out of nodes without finding the goal, so there's no solution
            return false;
        }

        protected void InitSearch(N startNode, N endNode, Heuristic<N> heuristic){
            metrics?.reset();

            // Increment the search id
            if (++_searchId < 0) _searchId = 1;

            // Initialize the open list
            _openList.Clear();

            // Initialize the record for the start node and add it to the open list
            NodeRecord<N> startRecord = GetNodeRecord(startNode);
            startRecord.node = startNode;
            startRecord.connection = null;
            startRecord.costSoFar = 0.ToLFloat();
            AddToOpenList(startRecord, heuristic.Estimate(startNode, endNode));

            _current = null;
        }

        protected void VisitChildren(N endNode, Heuristic<N> heuristic){
            // Get current node's outgoing connections
            var connections = _graph.GetConnections(_current.node);

            // Loop through each connection in turn
            for (int i = 0; i < connections.Count; i++) {
                if (metrics != null) metrics.VisitedNodes++;

                Connection<N> connection = connections[i];

                // Get the cost estimate for the node
                N node = connection.GetToNode(); //周围目标节点
                LFloat nodeCost = _current.costSoFar + connection.GetCost(); //节点到目标的消耗

                LFloat nodeHeuristic;
                NodeRecord<N> nodeRecord = GetNodeRecord(node);
                if (nodeRecord.category == CLOSED) { // The node is closed

                    // If we didn't find a shorter route, skip 已经是消耗最小的目标点
                    if (nodeRecord.costSoFar <= nodeCost) continue;

                    // We can use the node's old cost values to calculate its heuristic
                    // without calling the possibly expensive heuristic function
                    nodeHeuristic = nodeRecord.GetEstimatedTotalCost() - nodeRecord.costSoFar;
                }
                else if (nodeRecord.category == OPEN) { // The node is open

                    // If our route is no better, then skip
                    if (nodeRecord.costSoFar <= nodeCost) continue;

                    // Remove it from the open list (it will be re-added with the new cost)
                    _openList.Remove(nodeRecord);

                    // We can use the node's old cost values to calculate its heuristic
                    // without calling the possibly expensive heuristic function
                    nodeHeuristic = nodeRecord.GetEstimatedTotalCost() - nodeRecord.costSoFar;
                }
                else { // the node is unvisited

                    // We'll need to calculate the heuristic value using the function,
                    // since we don't have a node record with a previously calculated value
                    nodeHeuristic = heuristic.Estimate(node, endNode);
                }

                // Update node record's cost and connection
                nodeRecord.costSoFar = nodeCost;
                nodeRecord.connection = connection;

                // Add it to the open list with the estimated total cost
                AddToOpenList(nodeRecord, nodeCost + nodeHeuristic);
            }
        }

        protected void GeneratePath(N startNode, GraphPath<Connection<N>> outPath){
            // Work back along the path, accumulating connections
            // outPath.clear();
            while (!_current.node.Equals(startNode)) {
                outPath.Add(_current.connection);
                _current = _nodeRecords[_graph.GetIndex(_current.connection.GetFromNode())];
            }

            // Reverse the path
            outPath.reverse();
        }

        protected void GenerateNodePath(N startNode, GraphPath<N> outPath){
            // Work back along the path, accumulating nodes
            // outPath.clear();
            while (_current.connection != null) {
                outPath.Add(_current.node);
                _current = _nodeRecords[_graph.GetIndex(_current.connection.GetFromNode())];
            }

            outPath.Add(startNode);

            // Reverse the path
            outPath.reverse();
        }

        protected void AddToOpenList(NodeRecord<N> nodeRecord, LFloat estimatedTotalCost){
            _openList.Add(nodeRecord, estimatedTotalCost);
            nodeRecord.category = OPEN;
            if (metrics != null) {
                ++metrics.OpenListAdditions;
                if (_openList.size > metrics.OpenListPeak) metrics.OpenListPeak = _openList.size;
            }
        }

        protected NodeRecord<N> GetNodeRecord(N node){
            int index = _graph.GetIndex(node);
            NodeRecord<N> nr = _nodeRecords[index];
            if (nr != null) {
                if (nr.searchId != _searchId) {
                    nr.category = UNVISITED;
                    nr.searchId = _searchId;
                }

                return nr;
            }

            nr = _nodeRecords[index] = new NodeRecord<N>();
            nr.node = node;
            nr.searchId = _searchId;
            return nr;
        }

        public class NodeRecord<N> : Node {
            /** The reference to the node. */
            public N node;

            /** The incoming connection to the node */
            public Connection<N> connection;

            /** The actual cost from the start node. */
            public LFloat costSoFar;

            /** The node category: {@link #UNVISITED}, {@link #OPEN} or {@link #CLOSED}. */
            public int category;

            /** ID of the current search. */
            public int searchId;

            /** Creates a {@code NodeRecord}. */
            public NodeRecord() : base(LFloat.zero){ }

            /** Returns the estimated total cost. */
            public LFloat GetEstimatedTotalCost(){
                return value;
            }
        }

        public class Metrics {
            public int VisitedNodes;
            public int OpenListAdditions;
            public int OpenListPeak;

            public Metrics(){ }

            public void reset(){
                VisitedNodes = 0;
                OpenListAdditions = 0;
                OpenListPeak = 0;
            }
        }
    }
}