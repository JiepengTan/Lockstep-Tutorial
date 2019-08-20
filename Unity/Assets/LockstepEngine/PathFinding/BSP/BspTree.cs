//#define SHOW_BSP_TREE_GIZOMS
using System.Collections.Generic;
using System.Drawing;
using Lockstep.PathFinding;
using Lockstep.Collision2D;
using Lockstep.Math;
#if UNITY_EDITOR
using UnityEngine;
using Color = UnityEngine.Color;
#endif

namespace Lockstep.PathFinding {
    public class BspTree {
        public List<Triangle> allRawTriangle = new List<Triangle>();
        public List<TriRef> allTriangle = new List<TriRef>();

        public BspNode root;
        public static int maxDepth;
        public static int maxDepthNodeId;
        public void Init(List<Triangle> rawTriangles){
            this.allRawTriangle = rawTriangles;
            foreach (var tri in rawTriangles) {
                allTriangle.Add(new TriRef(tri));
            }

            root = new BspNode();
            root.Init(allTriangle);
#if UNITY_EDITOR && SHOW_BSP_TREE_GIZOMS
            var tran = DrawNode(root, 0, _debugTrans);
            tran.transform.position += Vector3.up * 0.05f;
#endif
        }

        public int GetTriangle(LVector3 pos){
            var pos2d = pos.ToLVector2XZ();
            return root.GetTriangle(pos2d);
        }
#if UNITY_EDITOR
        public static Transform _debugTrans;
        public static Material _debugMat;

        #region Debug Gizoms

        private static int worldSize = 100;

        Vector3 Hash13(int id){
            var val = (Mathf.Sin(15741.254f * id) + 1) / 2;
            var val2 = (Mathf.Sin(7331.5147f * id) + 1) / 2;
            var val3 = (Mathf.Sin(24317.433f * id) + 1) / 2;
            return new Vector3(val, val2, val3);
        }

        Transform DrawNode(BspNode node, int depth, Transform parent){
            var val = Hash13(node.nodeId);
            Color color = new Color(val.x, val.y, val.z, 1);
            if (node.IsLeaf) {
                var tran = CreateGo(node.nodeId.ToString(), node.tris, color).transform;
                tran.SetParent(parent, true);
                return tran;
            }
            else {
                var tran = CreateGo(node.nodeId.ToString(), node.SplitPlane, color).transform;
                tran.SetParent(parent, true);

                DrawNode(node.child[0], depth + 1, tran);
                DrawNode(node.child[1], depth + 1, tran);
                return tran;
            }
        }

        GameObject CreateGo(string name, SplitPlane plane, Color color){
            return new GameObject(name);
            var dir = plane.dir.normalized;
            var or = plane.a;
            var s = or - dir * worldSize;
            var e = or + dir * worldSize;
            var perp = new LVector2(-dir.y, dir.x);
            const float lineSize = 0.3f;
            var s1 = perp * lineSize.ToLFloat() + s;
            var e1 = perp * lineSize.ToLFloat() + e;
            List<Vector3> vertices = new List<Vector3>();
            List<int> tirs = new List<int>();
            vertices.Add(s.ToVector3XZ());
            vertices.Add(e.ToVector3XZ());
            vertices.Add(e1.ToVector3XZ());
            vertices.Add(s1.ToVector3XZ());
            int triIdx = 0;
            tirs.Add(0);
            tirs.Add(1);
            tirs.Add(2);
            tirs.Add(0);
            tirs.Add(2);
            tirs.Add(3);
            return CreateGo(name, vertices, tirs, color);
        }

        GameObject CreateGo(string name, List<TriRef> allRawTriangle, Color color){
            List<Vector3> vertices = new List<Vector3>();
            List<int> tirs = new List<int>();
            int triIdx = 0;
            foreach (var tri in allRawTriangle) {
                vertices.Add(tri.a.ToVector3XZ());
                vertices.Add(tri.b.ToVector3XZ());
                vertices.Add(tri.c.ToVector3XZ());
                tirs.Add(triIdx++);
                tirs.Add(triIdx++);
                tirs.Add(triIdx++);
            }

            return CreateGo(name, vertices, tirs, color);
        }

        GameObject CreateGo(string name, List<Vector3> vertices, List<int> tirs, Color color){
            var go = new GameObject(name);
            var render = go.AddComponent<MeshRenderer>();
            var mat = new Material(_debugMat) {color = color};
            render.material = mat;

            var tempMesh = new Mesh();
            tempMesh.vertices = vertices.ToArray();
            tempMesh.triangles = tirs.ToArray();
            go.AddComponent<MeshFilter>().mesh = tempMesh;
            return go;
        }

        #endregion

#endif
    }
}