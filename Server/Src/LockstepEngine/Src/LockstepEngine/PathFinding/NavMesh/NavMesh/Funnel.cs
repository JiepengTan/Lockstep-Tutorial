using Lockstep.Math;

namespace Lockstep.PathFinding {
    public class Funnel {

        public Plane leftPlane = new Plane(); // 左平面，高度为y轴
        public Plane rightPlane = new Plane();
        public LVector3 leftPortal = new LVector3(); // 路径左顶点，
        public LVector3 rightPortal = new LVector3(); // 路径右顶点
        public LVector3 pivot = new LVector3(); // 漏斗点，路径的起点或拐点

        public void setLeftPlane(LVector3 pivot, LVector3 leftEdgeVertex){
            leftPlane.set(pivot, pivot.Add(LVector3.up), leftEdgeVertex);
            leftPortal = leftEdgeVertex;
        }

        public void setRightPlane(LVector3 pivot, LVector3 rightEdgeVertex){
            rightPlane.set(pivot, pivot.Add(LVector3.up), rightEdgeVertex); // 高度
            rightPlane.normal = -rightPlane.normal; // 平面方向取反
            rightPlane.d = -rightPlane.d;
            rightPortal = (rightEdgeVertex);
        }

        public void setPlanes(LVector3 pivot, TriangleEdge edge){
            setLeftPlane(pivot, edge.leftVertex);
            setRightPlane(pivot, edge.rightVertex);
        }

        public PlaneSide sideLeftPlane(LVector3 point){
            return leftPlane.testPoint(point);
        }

        public PlaneSide sideRightPlane(LVector3 point){
            return rightPlane.testPoint(point);
        }
    }
}