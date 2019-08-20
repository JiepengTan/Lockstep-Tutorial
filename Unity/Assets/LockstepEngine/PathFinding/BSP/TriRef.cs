using System;
using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.PathFinding {
    public class TriRef {
        public int index; //正数为 原三角形，否则为
        public bool isSplit = false;

        public bool Contain(LVector2 pos){
            var isRightA = LVector2.Cross(b - a, pos - a) > 0;
            if (isRightA) return false;
            var isRightB = LVector2.Cross(c - b, pos - b) > 0;
            if (isRightB) return false;
            var isRightC = LVector2.Cross(a - c, pos - c) > 0;
            if (isRightC) return false;
            return true;
        }

        public TriRef(List<LVector2> vertexs, TriRef tri)
            : this(vertexs[0], vertexs[1], vertexs[2], tri){ }


        public TriRef(LVector2 a, LVector2 b, LVector2 c, TriRef tri)
            : this(a, b, c, tri.index){
            isSplit = true;
        }

        public TriRef(Triangle tri) : this(
            tri.a.ToLVector2XZ(),
            tri.b.ToLVector2XZ(),
            tri.c.ToLVector2XZ(),
            tri.index){ }

        public TriRef(LVector2 a, LVector2 b, LVector2 c, int idx){
            this.a = a;
            this.b = b;
            this.c = c;
            index = idx;

            borders = new SplitPlane[] {
                new SplitPlane(a, b),
                new SplitPlane(b, c),
                new SplitPlane(c, a)
            };
            //check valid 
            CheckValid();
        }

        void CheckValid(){
            for (int i = 0; i < 3; i++) {
                if (borders[i].dir == LVector2.zero) {
                    Debug.Assert(false);
                }
            }
        }

        public LVector2 a;
        public LVector2 b;
        public LVector2 c;

        public SplitPlane[] borders;

        public LVector2 this[int index] {
            get {
                switch (index) {
                    case 0: return a;
                    case 1: return b;
                    case 2: return c;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }
    }
}