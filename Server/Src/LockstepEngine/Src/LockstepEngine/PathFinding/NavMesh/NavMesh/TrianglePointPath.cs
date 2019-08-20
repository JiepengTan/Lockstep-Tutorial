using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public enum PlaneSide {
        OnPlane,
        Back,
        Front
    }

/**
 * A point where an edge on the navmesh is crossed.
 */
    public class TrianglePointPath {
        public static LVector3 V3_UP = LVector3.up;
        public static LVector3 V3_DOWN = LVector3.down;

        private Plane crossingPlane = new Plane(); // 横跨平面
        private static LVector3 tmp1 = new LVector3();
        private static LVector3 tmp2 = new LVector3();
        private List<Connection<Triangle>> nodes; // 路径连接点
        private LVector3 start; // 起点
        private LVector3 end; // 终点
        private Triangle startTri; // 起始三角形
        private EdgePoint lastPointAdded; // 最后一个边点
        private List<LVector3> vectors = new List<LVector3>(); // 路径坐标点
        private List<EdgePoint> pathPoints = new List<EdgePoint>();
        private TriangleEdge lastEdge; // 最后一个边


        public void CalculateForGraphPath(TriangleGraphPath trianglePath, bool calculateCrossPoint){
            Clear();
            nodes = trianglePath.nodes;
            start = trianglePath.start;
            end = trianglePath.end;
            startTri = trianglePath.startTri;

            // Check that the start point is actually inside the start triangle, if not,
            // project it to the closest
            // triangle edge. Otherwise the funnel calculation might generate spurious path
            // segments.
            Ray ray = new Ray((V3_UP.scl(1000.ToLFloat())).Add(start), V3_DOWN); // 起始坐标从上向下的射线
            if (!GeometryUtil.IntersectRayTriangle(ray, startTri.a, startTri.b, startTri.c, out var ss)) {
                LFloat minDst = LFloat.MaxValue;
                LVector3 projection = new LVector3(); // 规划坐标
                LVector3 newStart = new LVector3(); // 新坐标
                LFloat dst;
                // A-B
                if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startTri.a, startTri.b,
                        start)) < minDst) {
                    minDst = dst;
                    newStart.set(projection);
                }

                // B-C
                if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startTri.b, startTri.c,
                        start)) < minDst) {
                    minDst = dst;
                    newStart.set(projection);
                }

                // C-A
                if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startTri.c, startTri.a,
                        start)) < minDst) {
                    minDst = dst;
                    newStart.set(projection);
                }

                start.set(newStart);
            }

            if (nodes.Count == 0) { // 起点终点在同一三角形中
                addPoint(start, startTri);
                addPoint(end, startTri);
            }
            else {
                lastEdge = new TriangleEdge(nodes.get(nodes.Count - 1).GetToNode(),
                    nodes.get(nodes.Count - 1).GetToNode(),
                    end,
                    end);
                CalculateEdgePoints(calculateCrossPoint);
            }
        }

        public void Clear(){
            vectors.Clear();
            pathPoints.Clear();
            startTri = null;
            lastPointAdded = null;
            lastEdge = null;
        }

        private TriangleEdge getEdge(int index){
            return (TriangleEdge) ((index == nodes.Count) ? lastEdge : nodes[index]);
        }

        private int numEdges(){
            return nodes.Count + 1;
        }

        public LVector3 getVector(int index){
            return vectors.get(index);
        }

        public int getSize(){
            return vectors.Count;
        }

        /** All vectors in the path     */
        public List<LVector3> getVectors(){
            return vectors;
        }

        /** The triangle which must be crossed to reach the next path point.*/
        public Triangle getToTriangle(int index){
            return pathPoints.get(index).toNode;
        }

        /** The triangle from which must be crossed to reach this point. */
        public Triangle getFromTriangle(int index){
            return pathPoints.get(index).fromNode;
        }

        /** The navmesh edge(s) crossed at this path point.     */
        public List<TriangleEdge> getCrossedEdges(int index){
            return pathPoints.get(index).connectingEdges;
        }

        private void addPoint(LVector3 point, Triangle toNode){
            addPoint(new EdgePoint(point, toNode));
        }

        private void addPoint(EdgePoint edgePoint){
            vectors.Add(edgePoint.point);
            pathPoints.Add(edgePoint);
            lastPointAdded = edgePoint;
        }

        /**
         * Calculate the shortest
         * point path through the path triangles, using the Simple Stupid Funnel
         * Algorithm.
         *
         * @return
         */
        private void CalculateEdgePoints(bool calculateCrossPoint){
            TriangleEdge edge = getEdge(0);
            addPoint(start, edge.fromNode);
            lastPointAdded.fromNode = edge.fromNode;

            Funnel funnel = new Funnel();
            funnel.pivot = (start); // 起点为漏斗点
            funnel.setPlanes(funnel.pivot, edge); // 设置第一对平面

            int leftIndex = 0; // 左顶点索引
            int rightIndex = 0; // 右顶点索引
            int lastRestart = 0;

            for (int i = 1; i < numEdges(); ++i) {
                edge = getEdge(i); // 下一条边

                var leftPlaneLeftDP = funnel.sideLeftPlane(edge.leftVertex);
                var leftPlaneRightDP = funnel.sideLeftPlane(edge.rightVertex);
                var rightPlaneLeftDP = funnel.sideRightPlane(edge.leftVertex);
                var rightPlaneRightDP = funnel.sideRightPlane(edge.rightVertex);

                // 右顶点在右平面里面
                if (rightPlaneRightDP != PlaneSide.Front) {
                    // 右顶点在左平面里面
                    if (leftPlaneRightDP != PlaneSide.Front) {
                        // Tighten the funnel. 缩小漏斗
                        funnel.setRightPlane(funnel.pivot, edge.rightVertex);
                        rightIndex = i;
                    }
                    else {
                        // Right over left, insert left to path and restart scan from portal left point.
                        // 右顶点在左平面外面，设置左顶点为漏斗顶点和路径点，从新已该漏斗开始扫描
                        if (calculateCrossPoint) {
                            CalculateEdgeCrossings(lastRestart, leftIndex, funnel.pivot, funnel.leftPortal);
                        }
                        else {
                            vectors.Add(funnel.leftPortal);
                        }

                        funnel.pivot = (funnel.leftPortal);
                        i = leftIndex;
                        rightIndex = i;
                        if (i < numEdges() - 1) {
                            lastRestart = i;
                            funnel.setPlanes(funnel.pivot, getEdge(i + 1));
                            continue;
                        }

                        break;
                    }
                }

                // 左顶点在左平面里面
                if (leftPlaneLeftDP != PlaneSide.Front) {
                    // 左顶点在右平面里面
                    if (rightPlaneLeftDP != PlaneSide.Front) {
                        // Tighten the funnel.
                        funnel.setLeftPlane(funnel.pivot, edge.leftVertex);
                        leftIndex = i;
                    }
                    else {
                        // Left over right, insert right to path and restart scan from portal right
                        // point.
                        if (calculateCrossPoint) {
                            CalculateEdgeCrossings(lastRestart, rightIndex, funnel.pivot, funnel.rightPortal);
                        }
                        else {
                            vectors.Add(funnel.rightPortal);
                        }

                        funnel.pivot = (funnel.rightPortal);
                        i = rightIndex;
                        leftIndex = i;
                        if (i < numEdges() - 1) {
                            lastRestart = i;
                            funnel.setPlanes(funnel.pivot, getEdge(i + 1));
                            continue;
                        }

                        break;
                    }
                }
            }

            if (calculateCrossPoint) {
                CalculateEdgeCrossings(lastRestart, numEdges() - 1, funnel.pivot, end);
            }
            else {
                vectors.Add(end);
            }

            for (int i = 1; i < pathPoints.Count; i++) {
                EdgePoint p = pathPoints.get(i);
                p.fromNode = pathPoints.get(i - 1).toNode;
            }

            return;
        }

        /**
         * Store all edge crossing points between the start and end indices. If the path
         * crosses exactly the start or end points (which is quite likely), store the
         * edges in order of crossing in the EdgePoint data structure.
         * <p/>
         * Edge crossings are calculated as intersections with the plane from the start,
         * end and up vectors.
         */
        private void CalculateEdgeCrossings(int startIndex, int endIndex, LVector3 startPoint, LVector3 endPoint){
            if (startIndex >= numEdges() || endIndex >= numEdges()) {
                return;
            }

            crossingPlane.set(startPoint, tmp1.set(startPoint).Add(V3_UP), endPoint);

            EdgePoint previousLast = lastPointAdded;

            var edge = getEdge(endIndex);
            EdgePoint end = new EdgePoint(endPoint, edge.toNode);

            for (int i = startIndex; i < endIndex; i++) {
                edge = getEdge(i);

                if (edge.rightVertex.Equals(startPoint) || edge.leftVertex.Equals(startPoint)) {
                    previousLast.toNode = edge.toNode;
                    if (!previousLast.connectingEdges.Contains(edge)) {
                        previousLast.connectingEdges.Add(edge);
                    }
                }
                else if (edge.leftVertex.Equals(endPoint) || edge.rightVertex.Equals(endPoint)) {
                    if (!end.connectingEdges.Contains(edge)) {
                        end.connectingEdges.Add(edge);
                    }
                }
                else if (IntersectSegmentPlane(edge.leftVertex, edge.rightVertex, crossingPlane, tmp1)) {
                    if (i != startIndex || i == 0) {
                        lastPointAdded.toNode = edge.fromNode;
                        EdgePoint crossing = new EdgePoint(tmp1, edge.toNode);
                        crossing.connectingEdges.Add(edge);
                        addPoint(crossing);
                    }
                }
            }

            if (endIndex < numEdges() - 1) {
                end.connectingEdges.Add(getEdge(endIndex));
            }

            if (!lastPointAdded.Equals(end)) {
                addPoint(end);
            }
        }

        public static bool IntersectSegmentPlane(LVector3 start, LVector3 end, Plane plane, LVector3 intersection){
            LVector3 dir = end.sub(start);
            LFloat denom = dir.dot(plane.getNormal());
            LFloat t = -(start.dot(plane.getNormal()) + plane.getD()) / denom;
            if (t < 0 || t > 1)
                return false;

            intersection.set(start).Add(dir.scl(t));
            return true;
        }
    }
}