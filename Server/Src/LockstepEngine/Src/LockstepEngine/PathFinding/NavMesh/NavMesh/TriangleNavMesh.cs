using System;
using System.Collections.Generic;
using Lockstep.Math;
using Lockstep.Util;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.PathFinding {
    public class TriangleNavMesh : NavMesh {
        public TriangleGraph _graph;
        private TriangleHeuristic _heuristic;
        public IndexedAStarPathFinder<Triangle> _pathFinder;

        public TriangleNavMesh(String navMeshStr) : this(navMeshStr, 1){ }
        public BspTree bspTree => _graph.bspTree;
        public TriangleNavMesh(String navMeshStr, int scale){
            var data = JsonUtil.ToObject<TriangleData>(navMeshStr);
            _graph = new TriangleGraph(data, scale);
            _pathFinder = new IndexedAStarPathFinder<Triangle>(_graph, true);
            _heuristic = new TriangleHeuristic();
        }

        public TriangleGraphPath navMeshGraphPath = null;

        public List<LVector3> FindPath(LVector3 fromPoint, LVector3 toPoint, TrianglePointPath navMeshPointPath){
            navMeshGraphPath = new TriangleGraphPath();
            bool find = FindPath(fromPoint, toPoint, navMeshGraphPath);
            if (!find) {
                return null;
            }

            navMeshPointPath.CalculateForGraphPath(navMeshGraphPath, false);
            return navMeshPointPath.getVectors();
        }

        private bool FindPath(LVector3 fromPoint, LVector3 toPoint, TriangleGraphPath path){
            path.Clear();
            Triangle fromTriangle = GetTriangle(fromPoint);
            var toTriangle = GetTriangle(toPoint);
            if (_pathFinder.SearchPath(fromTriangle, toTriangle, _heuristic, path)) {
                path.start = fromPoint;
                path.end = toPoint;
                path.startTri = fromTriangle;
                return true;
            }

            return false;
        }

        public TriangleGraph GetGraph(){
            return _graph;
        }

        public TriangleHeuristic GetHeuristic(){
            return _heuristic;
        }

        public IndexedAStarPathFinder<Triangle> GetPathFinder(){
            return _pathFinder;
        }


        public Triangle GetTriangle(LVector3 point){
            return _graph.GetTriangle(point);
        }
    }
}