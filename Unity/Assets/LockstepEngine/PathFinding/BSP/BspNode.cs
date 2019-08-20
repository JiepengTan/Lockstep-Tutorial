using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    public class BspNode {
        public List<TriRef> tris;
        public BspNode[] child;

        public int nodeId;
        public static int _debugNodeId = 0;

        public BspNode(){
            nodeId = BspNode._debugNodeId++;
        }

        public SplitPlane SplitPlane;
        public bool IsLeaf => child == null;
        public const int ChildCount = 2;
        public int minTriCount = 3;
        public int depth = 0;

        static HashSet<int> uniqueId = new HashSet<int>();

        public BspNode leftChild => child[0];
        public BspNode rightChild => child[1];

        static int maxDepth {
            get => BspTree.maxDepth;
            set => BspTree.maxDepth = value;
        }
        static int maxDepthNodeId {
            get => BspTree.maxDepthNodeId;
            set => BspTree.maxDepthNodeId = value;
        }
        public int GetTriangle(LVector2 pos){
            if (IsLeaf) {
                foreach (var tri in tris) {
                    if (tri.Contain(pos)) {
                        return tri.index;
                    }
                }

                return -1;
            }
            else {
                var isLeft = LVector2.Cross(SplitPlane.b - SplitPlane.a, pos - SplitPlane.a) >= 0;
                if (isLeft) {
                    return leftChild.GetTriangle(pos);
                }
                else {
                    return rightChild.GetTriangle(pos);
                }
            }
        }

        public void Init(List<TriRef> tris, int depth = 0){
            uniqueId.Clear();
            foreach (var tri in tris) {
                uniqueId.Add(tri.index);
            }

            if (depth > maxDepth) {
                maxDepth = depth;
                maxDepthNodeId = nodeId;
            }

            this.depth = depth;
            this.tris = tris;
            if (nodeId == 12) {
                int i = 0;
            }

            if (uniqueId.Count <= minTriCount) {
                return;
            }
            if (depth > 20)
                return;
            SplitPlane = PickSplittingPlane(tris);
            child = new BspNode[] {
                new BspNode(),
                new BspNode()
            };
            var lTris = new List<TriRef>();
            var rTris = new List<TriRef>();
            foreach (var tri in tris) {
                var val = SplitPlane.GetSplitResult(SplitPlane, tri);
                if (val == ESplitType.Left)
                    lTris.Add(tri);
                else if (val == ESplitType.Right)
                    rTris.Add(tri);
                else {
                    //split triangle into muti polygon
                    SplitTri(lTris, rTris, tri);
                }
            }

            leftChild.Init(lTris, depth + 1);
            rightChild.Init(rTris, depth + 1);
        }

        List<LVector2> rVerts = new List<LVector2>();
        List<LVector2> lVerts = new List<LVector2>();

        private void SplitTri(List<TriRef> lTris, List<TriRef> rTris, TriRef tri){
            int numVerts = 3;
            rVerts.Clear();
            lVerts.Clear();
            var a = tri[2];
            var aSide = SplitPlane.ClassifyPointToPlane(SplitPlane, a);
            for (int i = 0; i < numVerts; i++) {
                var b = tri[i];
                var bSide = SplitPlane.ClassifyPointToPlane(SplitPlane, b);
                if (bSide == ESplitType.Right) {
                    if (aSide == ESplitType.Left) {
                        var p = SplitPlane.GetIntersectPoint(a, b, SplitPlane.a, SplitPlane.b);
                        rVerts.Add(p);
                        lVerts.Add(p);
                    }
                    else if (aSide == ESplitType.OnPlane) {
                        if (rVerts.Count ==0 || a != rVerts[rVerts.Count - 1]) {
                            rVerts.Add(a);
                        }
                    }

                    rVerts.Add(b);
                }
                else if (bSide == ESplitType.Left) {
                    if (aSide == ESplitType.Right) {
                        var p = SplitPlane.GetIntersectPoint(a, b, SplitPlane.a, SplitPlane.b);
                        rVerts.Add(p);
                        lVerts.Add(p);
                    }
                    else if (aSide == ESplitType.OnPlane) {
                        if (lVerts.Count ==0 || a != lVerts[lVerts.Count - 1]) {
                            lVerts.Add(a);
                        }
                    }

                    lVerts.Add(b);
                }
                else {
                    if (aSide == ESplitType.Right) {
                        if (!(rVerts.Count == 3 && b == rVerts[0])) {
                            rVerts.Add(b);
                        }
                    }
                    else if (aSide == ESplitType.Left) {
                        if (!(lVerts.Count == 3 && b == lVerts[0])) {
                            lVerts.Add(b);
                        }
                    }
                }

                a = b;
                aSide = bSide;
            }

            //push into tri list
            if (lVerts.Count >= 3) {
                if (lVerts.Count == 3) {
                    AddTriangle(lTris,lVerts[0], lVerts[1], lVerts[2], tri);
                }
                else {
                    Debug.Assert(lVerts.Count == 4);
                    AddTriangle(lTris,lVerts[0], lVerts[1], lVerts[2], tri);
                    AddTriangle(lTris,lVerts[0], lVerts[2], lVerts[3], tri);
                }
            }

            if (rVerts.Count >= 3) {
                if (rVerts.Count == 3) {
                    AddTriangle(rTris,rVerts[0], rVerts[1], rVerts[2], tri);
                }
                else {
                    Debug.Assert(rVerts.Count == 4);
                    AddTriangle(rTris,rVerts[0], rVerts[1], rVerts[2], tri);
                    AddTriangle(rTris,rVerts[0], rVerts[2], rVerts[3], tri);
                }
            }
        }

        void AddTriangle(List<TriRef> rTris,LVector2 a, LVector2 b, LVector2 c, TriRef tri){
            if(a == b || b == c || c ==a)
                return;
            rTris.Add(new TriRef(a,b,c, tri));;
        }


        static SplitPlane PickSplittingPlane(List<TriRef> tris){
            int minScore = int.MinValue;
            int minLCount = 0;
            int minRCount = 0;
            SplitPlane bestPlane = new SplitPlane();
            int[] splitCounter = new int[(int) ESplitType.EnumCount];
            var tirCount = tris.Count;
            foreach (var tri in tris) {
                foreach (var plane in tri.borders) {
                    for (int i = 0; i < (int) ESplitType.EnumCount; i++) {
                        splitCounter[i] = 0;
                    }

                    foreach (var otherTri in tris) {
                        var val = (int) SplitPlane.GetSplitResult(plane, otherTri);
                        splitCounter[val]++;
                    }

                    // Clock wise =>Left ++   CCW=>Right++// self tri is on the left
                    var leftCount = splitCounter[(int) ESplitType.Left];
                    var rightCount = splitCounter[(int) ESplitType.Right];
                    var splitCount = splitCounter[(int) ESplitType.OnPlane];
                    if ((leftCount == 0 || rightCount == 0) && leftCount + rightCount == tirCount) {
                        continue;
                    }

                    var balanceVal = LMath.Abs(leftCount - rightCount);
                    var sameVal = LMath.Min(leftCount, rightCount);
                    int score = sameVal * 3 - balanceVal - splitCount * 2;

                    if (score > minScore) {
                        minLCount = leftCount;
                        minRCount = rightCount;
                        minScore = score;
                        bestPlane = plane;
                    }
                }
            }

            return bestPlane;
        }
    }
}