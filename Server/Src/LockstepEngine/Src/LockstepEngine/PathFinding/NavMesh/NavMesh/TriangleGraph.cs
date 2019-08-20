using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Util;

namespace Lockstep.PathFinding {
    public class TriangleGraph : IndexedGraph<Triangle> {
        private NavMeshData _navMeshData;
        public List<Triangle> _triangles = new List<Triangle>();

        private Dictionary<Triangle, List<Connection<Triangle>>> _sharedEdges;
        private Dictionary<Triangle, List<Connection<Triangle>>> _isolatedEdgesMap;

        private Dictionary<int,Triangle> _id2Tri = new Dictionary<int, Triangle>();
        private int _numDisconnectedEdges;
        private int _numConnectedEdges;
        private int _numTotalEdges;

        public BspTree bspTree;
        public TriangleGraph(NavMeshData navMeshData, int scale){
            this._navMeshData = navMeshData;
            navMeshData.check(scale);
            var pathTriangles = CreateTriangles(scale);
            foreach (var triangle in pathTriangles) {
                _id2Tri[triangle.index] = triangle;
            }
            var pathIndexConnections = GetIndexConnections(navMeshData.GetPathTriangles());
            _sharedEdges =
                CreateSharedEdgesMap(pathIndexConnections, pathTriangles, navMeshData.GetPathVertices());
            _isolatedEdgesMap = CreateIsolatedEdgesMap(_sharedEdges);
            bspTree = new BspTree();
            bspTree.Init(pathTriangles);
            // Count edges of different types
            foreach (var edges in _isolatedEdgesMap.Values) {
                _numDisconnectedEdges += edges.Count;
            }

            foreach (var edges in _sharedEdges.Values) {
                _numConnectedEdges += edges.Count;
            }

            _numConnectedEdges /= 2;
            _numTotalEdges = _numConnectedEdges + _numDisconnectedEdges;
            Debug.Log(
                $"mapId{navMeshData.getMapID()} triangles{getTriangleCont()} totalEdges{_numTotalEdges} connEdges{_numConnectedEdges} disConnEdges{_numDisconnectedEdges}");
        }

        public List<Connection<Triangle>> GetConnections(Triangle fromNode){
            return _sharedEdges.TryGetValue(fromNode, out var val) ? val : null;
        }

        public int GetIndex(Triangle node){
            return node.getIndex();
        }

        public int GetNodeCount(){
            return _sharedEdges.Count;
        }

        public NavMeshData getNavMeshData(){
            return _navMeshData;
        }

        private List<Triangle> CreateTriangles(int scale){
            var vertexIndexs = _navMeshData.GetPathTriangles();
            var vertices = _navMeshData.GetPathVertices();
            int triangleIndex = 0; // 三角形下标
            int length = vertexIndexs.Length - 3;
            for (int i = 0; i <= length;) {
                int aIndex = vertexIndexs[i++];
                int bIndex = vertexIndexs[i++];
                int cIndex = vertexIndexs[i++];
                try {
                    Triangle triangle = null;
                    if (scale != 1) {
                        triangle = new Triangle(vertices[aIndex], vertices[bIndex], vertices[cIndex], triangleIndex++,
                            aIndex,
                            bIndex, cIndex);
                    }
                    else {
                        triangle = new Triangle(vertices[aIndex], vertices[bIndex], vertices[cIndex], triangleIndex++);
                    }

                    _triangles.Add(triangle);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    throw;
                }


            }

            return _triangles;
        }

        private static HashSet<IndexConnection> GetIndexConnections(int[] indices){
            var indexConnections = new HashSet<IndexConnection>();
            int[] edge = {-1, -1};
            short i = 0;
            int j, a0, a1, a2, b0, b1, b2, triAIndex, triBIndex;
            while (i < indices.Length) {
                triAIndex = (short) (i / 3); // A三角形编号
                a0 = indices[i++];
                a1 = indices[i++];
                a2 = indices[i++];
                j = i;
                while (j < indices.Length) {
                    triBIndex = (short) (j / 3); // B三角形编号
                    b0 = indices[j++];
                    b1 = indices[j++];
                    b2 = indices[j++];

                    if (HasSharedEdgeIndices(a0, a1, a2, b0, b1, b2, edge)) {
                        var indexConnection1 = new IndexConnection(edge[0], edge[1], triAIndex, triBIndex);
                        var indexConnection2 = new IndexConnection(edge[1], edge[0], triBIndex, triAIndex);
                        indexConnections.Add(indexConnection1);
                        indexConnections.Add(indexConnection2);
                        edge[0] = -1;
                        edge[1] = -1;
                        // Debug.LogError("共享边：{} ->
                        // {}",indexConnection1.ToString(),indexConnection2.ToString());
                    }
                }
            }

            Debug.Log($"连接个数：{indexConnections.Count}");
            return indexConnections;
        }

        /** Checks if the two triangles have shared vertex indices. The edge
         * will always follow the vertex winding order of the triangle A. Since all
         * triangles have the same winding order, triangle A should have the opposite
         * edge direction to triangle B.
         */
        private static bool HasSharedEdgeIndices(int a0, int a1, int a2, int b0, int b1, int b2, int[] edge){
            bool match0 = (a0 == b0 || a0 == b1 || a0 == b2);
            bool match1 = (a1 == b0 || a1 == b1 || a1 == b2);
            if (!match0 && !match1) { // 无两个共享点
                return false;
            }
            else if (match0 && match1) {
                edge[0] = a0;
                edge[1] = a1;
                return true;
            }

            bool match2 = (a2 == b0 || a2 == b1 || a2 == b2);
            if (match0 && match2) {
                edge[0] = a2;
                edge[1] = a0;
                return true;
            }
            else if (match1 && match2) {
                edge[0] = a1;
                edge[1] = a2;
                return true;
            }

            return false;
        }

        private static Dictionary<Triangle, List<Connection<Triangle>>> CreateSharedEdgesMap(
            HashSet<IndexConnection> indexConnections, List<Triangle> triangles, LVector3[] vertexVectors){
            var connectionMap = new Dictionary<Triangle, List<Connection<Triangle>>>();

            foreach (Triangle tri in triangles) {
                connectionMap.Add(tri, new List<Connection<Triangle>>());
            }

            foreach (IndexConnection indexConnection in indexConnections) {
                var fromNode = triangles.get(indexConnection.fromTriIndex);
                var toNode = triangles.get(indexConnection.toTriIndex);
                var edgeVertexA = vertexVectors[indexConnection.edgeVertexIndex1];
                var edgeVertexB = vertexVectors[indexConnection.edgeVertexIndex2];

                var edge = new TriangleEdge(fromNode, toNode, edgeVertexA, edgeVertexB);
                connectionMap.get(fromNode).Add(edge);
                fromNode.connections.Add(edge);
                Debug.LogFormat( $"Triangle：{fromNode.getIndex()} -->{toNode.getIndex()} {fromNode}-->{toNode}");
            }

            return connectionMap;
        }


        /**
         * 存储相互连接三角形的关系 Class for storing the edge connection data between two adjacent
         * triangles.
         */
        public class IndexConnection {
            // The vertex indices which makes up the edge shared between two triangles.
            public int edgeVertexIndex1;

            public int edgeVertexIndex2;

            // The indices of the two triangles sharing this edge.
            public int fromTriIndex;
            public int toTriIndex;

            public IndexConnection(int sharedEdgeVertex1Index, int edgeVertexIndex2, int fromTriIndex, int toTriIndex){
                this.edgeVertexIndex1 = sharedEdgeVertex1Index;
                this.edgeVertexIndex2 = edgeVertexIndex2;
                this.fromTriIndex = fromTriIndex;
                this.toTriIndex = toTriIndex;
            }

            public override String ToString(){
                return "IndexConnection [edgeVertexIndex1=" + edgeVertexIndex1 + ", edgeVertexIndex2=" +
                       edgeVertexIndex2
                       + ", fromTriIndex=" + fromTriIndex + ", toTriIndex=" + toTriIndex + "]";
            }

            public override int GetHashCode(){
                int prime = 31;
                int result = 1;
                result = prime * result + edgeVertexIndex1;
                result = prime * result + edgeVertexIndex2;
                result = prime * result + fromTriIndex;
                result = prime * result + toTriIndex;
                return result;
            }

            public override bool Equals(object obj){
                if (this == obj)
                    return true;
                if (obj == null)
                    return false;
                if (GetType() != obj.GetType())
                    return false;
                IndexConnection other = (IndexConnection) obj;
                if (edgeVertexIndex1 != other.edgeVertexIndex1)
                    return false;
                if (edgeVertexIndex2 != other.edgeVertexIndex2)
                    return false;
                if (fromTriIndex != other.fromTriIndex)
                    return false;
                if (toTriIndex != other.toTriIndex)
                    return false;
                return true;
            }
        }

        public Dictionary<Triangle, List<Connection<Triangle>>> getPathSharedEdges(){
            return _sharedEdges;
        }

        /**获取所有三角形列表     */
        public List<Triangle> getTriangles(){
            return _triangles;
        }

        public Triangle GetTriangle(LVector3 point){
            //TODO space partition bsp
            Profiler.BeginSample("_GetTriangle");
            var ret = _GetTriangle(point);
            Profiler.EndSample();
            return ret;
        }

        private Triangle _GetTriangle(LVector3 point){
            var triId = bspTree.GetTriangle(point);
            if (_id2Tri.TryGetValue(triId, out var tri)) {
                return tri;
            }
            //foreach (var triangle in _triangles) {
            //    if (triangle.IsInnerPoint(point)) {
            //        return triangle;
            //    }
            //}
            return null;
        }

        private static Dictionary<Triangle, List<Connection<Triangle>>> CreateIsolatedEdgesMap(
            Dictionary<Triangle, List<Connection<Triangle>>> connectionMap){
            var disconnectionMap = new Dictionary<Triangle, List<Connection<Triangle>>>();

            foreach (Triangle tri in connectionMap.Keys) {
                var connectedEdges = connectionMap.get(tri);

                var disconnectedEdges = new List<Connection<Triangle>>();
                disconnectionMap.Add(tri, disconnectedEdges);

                if (connectedEdges.Count < 3) {
                    // This triangle does not have all edges connected to other triangles
                    bool ab = true;
                    bool bc = true;
                    bool ca = true;
                    foreach (var item in connectedEdges) {
                        var edge = item as TriangleEdge;
                        if (edge.rightVertex == tri.a && edge.leftVertex == tri.b)
                            ab = false;
                        else if (edge.rightVertex == tri.b && edge.leftVertex == tri.c)
                            bc = false;
                        else if (edge.rightVertex == tri.c && edge.leftVertex == tri.a)
                            ca = false;
                    }

                    if (ab)
                        disconnectedEdges.Add(new TriangleEdge(tri, null, tri.a, tri.b));
                    if (bc)
                        disconnectedEdges.Add(new TriangleEdge(tri, null, tri.b, tri.c));
                    if (ca)
                        disconnectedEdges.Add(new TriangleEdge(tri, null, tri.c, tri.a));
                }

                int totalEdges = (connectedEdges.Count + disconnectedEdges.Count);
                if (totalEdges != 3) {
                    Debug.LogError("Wrong number of edges (" + totalEdges + ") in triangle " +
                                               tri.getIndex());
                }
            }

            return disconnectionMap;
        }

        public int getNumDisconnectedEdges(){
            return _numDisconnectedEdges;
        }

        public int getNumConnectedEdges(){
            return _numConnectedEdges;
        }

        public int getNumTotalEdges(){
            return _numTotalEdges;
        }

        public int getTriangleCont(){
            return _triangles.Count;
        }
    }
}