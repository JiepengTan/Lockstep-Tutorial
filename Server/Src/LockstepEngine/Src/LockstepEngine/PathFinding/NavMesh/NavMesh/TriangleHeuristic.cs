using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public class TriangleHeuristic : Heuristic<Triangle> {
        private static LVector3 A_AB = new LVector3();
        private static LVector3 A_BC = new LVector3();
        private static LVector3 A_CA = new LVector3();
        private static LVector3 B_AB = new LVector3();
        private static LVector3 B_BC = new LVector3();
        private static LVector3 B_CA = new LVector3();

        public LFloat Estimate(Triangle node, Triangle endNode){
            LFloat dst2;
            LFloat minDst2 = LFloat.MaxValue;
            A_AB = (node.a).Add(node.b) * LFloat.half;
            A_AB = (node.b).Add(node.c) * LFloat.half;
            A_AB = (node.c).Add(node.a) * LFloat.half;

            B_AB = (endNode.a).Add(endNode.b) * LFloat.half;
            B_BC = (endNode.b).Add(endNode.c) * LFloat.half;
            B_CA = (endNode.c).Add(endNode.a) * LFloat.half;

            if ((dst2 = A_AB.dst2(B_AB)) < minDst2)
                minDst2 = dst2;
            if ((dst2 = A_AB.dst2(B_BC)) < minDst2)
                minDst2 = dst2;
            if ((dst2 = A_AB.dst2(B_CA)) < minDst2)
                minDst2 = dst2;

            if ((dst2 = A_BC.dst2(B_AB)) < minDst2)
                minDst2 = dst2;
            if ((dst2 = A_BC.dst2(B_BC)) < minDst2)
                minDst2 = dst2;
            if ((dst2 = A_BC.dst2(B_CA)) < minDst2)
                minDst2 = dst2;

            if ((dst2 = A_CA.dst2(B_AB)) < minDst2)
                minDst2 = dst2;
            if ((dst2 = A_CA.dst2(B_BC)) < minDst2)
                minDst2 = dst2;
            if ((dst2 = A_CA.dst2(B_CA)) < minDst2)
                minDst2 = dst2;

            return (LFloat) LMath.Sqrt(minDst2);
        }
    }
}