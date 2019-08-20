#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lockstep.Math;
using Lockstep.PathFinding;
using Lockstep.Util;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using Object = UnityEngine.Object;

[CustomEditor(typeof(NavMeshTool))]
public class EditorNavMeshTool : UnityEditor.Editor {
    private GameObject _map;
    private GameObject _walkLayer;
    private TriangleData _property; //地图存储属性
    private NavMeshTool owner;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = (target as NavMeshTool);
        _property = owner?.data;
        EditorGUILayoutExt.FloatField("地图宽度", _property.endX - _property.startX);
        EditorGUILayoutExt.FloatField("地图高度", _property.endZ - _property.startZ);
        if (GUILayout.Button("测试地图大小")) {
            CreateMapTestMesh();
        }

        if (_property.mapID == -1) {
            CheckInit();
        }

        EditorGUILayout.Separator();
        if (GUILayout.Button("生成寻路数据")) {
            CreateNavMeshData();
        }

        EditorGUILayout.Separator();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private bool CheckInit(){
        ClearTmp();
        _map = owner.mapRoot;
        _walkLayer = owner.mapRoot;
        _property.mapID = owner.mapId;
        return true;
    }

    /// <summary>
    /// 创建测试地图大小
    /// </summary>
    void CreateMapTestMesh(){
        CheckInit();
        _map.SetActive(true);
        GameObject UnWalkAble = CreateOb("MapTest", 0, new Mesh());
        Mesh UnWalkMesh = UnWalkAble.GetComponent<MeshFilter>().sharedMesh;
        UnWalkMesh.vertices = new Vector3[] {
            new LVector3(_property.startX, LFloat.zero, _property.startZ).ToVector3(),
            new LVector3(_property.startX, LFloat.zero, _property.endZ + _property.startZ).ToVector3(),
            new LVector3(_property.endX + _property.startX, LFloat.zero, _property.endZ + _property.startZ).ToVector3(),
            new LVector3(_property.endX + _property.startX, LFloat.zero, _property.startZ).ToVector3()
        };
        UnWalkMesh.triangles = new int[] {0, 1, 2, 0, 2, 3};
    }

    /// <summary>
    /// 清除临时属性
    /// </summary>
    void ClearTmp(){
        GameObject MapNavMeshResult = GameObject.Find("MapNavMeshResult");
        if (MapNavMeshResult) {
            Object.DestroyImmediate(MapNavMeshResult);
        }

        GameObject MapTest = GameObject.Find("MapTest");
        if (MapTest) {
            Object.DestroyImmediate(MapTest);
        }

        GameObject NavMesh_WalkAble = GameObject.Find("NavMesh_WalkAble");
        if (NavMesh_WalkAble) {
            Object.DestroyImmediate(NavMesh_WalkAble);
        }

        GameObject NavMesh_UnWalkAble = GameObject.Find("NavMesh_UnWalkAble");
        if (NavMesh_UnWalkAble) {
            Object.DestroyImmediate(NavMesh_UnWalkAble);
        }
    }

    private static DateTime startTime;

    static void LogElapseTime(string tag = ""){
        var ms = (DateTime.Now - startTime).TotalMilliseconds;
        Debug.LogWarning($"{tag} use time:{ms} ms");
        startTime = DateTime.Now;
    }


    /// <summary>
    /// 创建navmesh数据
    /// </summary>
    void CreateNavMeshData(){
        CheckInit();
        startTime = DateTime.Now;
        _map.SetActive(false);

        //UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
        //UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        LogElapseTime("BuildNavMesh");
        string path = Path.Combine(Application.dataPath, "Resources/Maps/");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        var triangulatedNavMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        foreach (var vertex in triangulatedNavMesh.vertices) {
            if (vertex.x > maxX) maxX = vertex.x;
            if (vertex.x < minX) minX = vertex.x;
            if (vertex.z > maxZ) maxZ = vertex.z;
            if (vertex.z < minZ) minZ = vertex.z;
        }

        _property.pathTriangles = triangulatedNavMesh.indices;
        _property.startX = minX.ToLFloat();
        _property.startZ = minZ.ToLFloat();
        _property.endX = maxX.ToLFloat();
        _property.endZ = maxZ.ToLFloat();
        _property.width  = (maxX - minX).ToLFloat();
        _property.height = (maxZ - minZ).ToLFloat();

        MergeVertices(triangulatedNavMesh.vertices);
        var strs = JsonUtil.ToJson(_property);
        LogElapseTime("Build str");
        string filename = path + _property.mapID + ".navmesh.json";
        MeshToFile(filename, strs);
        LogElapseTime("MeshToFile");
        _map.SetActive(true);
        alert("成功！");

        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        ShowNavMesh(triangulatedNavMesh);
    }

    void MergeVertices(Vector3[] pathVertices){
        var rawCount = pathVertices.Length;
        double minGap = 0.05f;
        var hashSet = new Dictionary<double, List<Vector3>>();
        var rawVertices = pathVertices;

        double Hash31(Vector3 vec){
            return ((long) (((double) vec.sqrMagnitude) / minGap) * minGap);
        }

        bool CanMerge(double hash, Vector3 vertex){
            bool canMerge = false;
            for (int j = -1; j <= 1; j++) {
                var nearHash = hash + minGap * j;
                if (hashSet.TryGetValue(nearHash, out var lst)) {
                    foreach (var ver in lst) {
                        if ((ver - vertex).sqrMagnitude < minGap) {
                            canMerge = true;
                            break;
                        }
                    }
                }

                if (canMerge) break;
            }

            return canMerge;
        }

        for (int i = 0; i < rawVertices.Length; i++) {
            var vertex = rawVertices[i];  
            var hash = Hash31(vertex);
            bool canMerge = CanMerge(hash, vertex);
            if (!canMerge) {
                if (hashSet.TryGetValue(hash, out var lst)) {
                    lst.Add(vertex);
                }
                else {
                    hashSet.Add(hash, new List<Vector3>() {vertex});
                }
            }
        }

        var newVertices = new List<Vector3>();
        var pos2Idx = new Dictionary<Vector3, int>();
        int posIds = 0;
        foreach (var pair in hashSet) {
            foreach (var vec in pair.Value) {
                newVertices.Add(vec);
                pos2Idx.Add(vec, posIds++);
            }
        }

        _property.pathVertices = newVertices.ToArray().ToLVecArray();
        var rawIdxs = _property.pathTriangles;
        var newIdxs = new int[_property.pathTriangles.Length];
        for (int i = 0; i < rawIdxs.Length; i++) {
            var rawVertex = rawVertices[rawIdxs[i]];
            var hash = Hash31(rawVertex);
            bool merged = false;
            for (int j = -1; j <= 1; j++) {
                var nearHash = hash + minGap * j;
                if (hashSet.TryGetValue(nearHash, out var lst)) {
                    foreach (var ver in lst) {
                        if ((ver - rawVertex).magnitude < minGap) {
                            newIdxs[i] = pos2Idx[ver];
                            merged = true;
                            break;
                        }
                    }
                }

                if (merged) break;
            }

            if (!merged) {
                Debug.LogError($"hehe  can not find merge point" + rawVertex );
            }
        }
        //check the same
        for (int i = 0; i < rawIdxs.Length; i++) {
            var rawPos = rawVertices[rawIdxs[i]];
            var newPos = newVertices[newIdxs[i]];
            if (rawPos != newPos) {
                var diff = (rawPos - newPos).sqrMagnitude;
                if (diff > 0.01f) {
                    Debug.LogError("Miss match pos rawPos:{rawPos} newPos:{newPos} diff = " + diff);
                }
            }
        }

        _property.pathTriangles = newIdxs;
        UnityEngine.Debug.Log($"MergeVertices {rawCount}->{_property.pathVertices.Length}");
    }

    private static void ShowNavMesh(NavMeshTriangulation triangulatedNavMesh){
        var MapNavMeshResult = new Mesh();
        MapNavMeshResult.vertices = triangulatedNavMesh.vertices;
        MapNavMeshResult.triangles = triangulatedNavMesh.indices;
        MapNavMeshResult.RecalculateBounds();
        GameObject ob = GameObject.Find("MapNavMeshResult");
        if (ob == null) {
            ob = new GameObject("MapNavMeshResult");
            ob.AddComponent<MeshFilter>().mesh = MapNavMeshResult; //网格
            ob.AddComponent<MeshRenderer>(); //网格渲染器  
        }
    }

    private void alert(string content){
        EditorWindow.focusedWindow.ShowNotification(new GUIContent(content));
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="WalkLayer"></param>
    /// <returns></returns>
    private GameObject CreateOb(string name, int WalkLayer, Mesh walkMesh){
        GameObject ob = GameObject.Find(name);
        walkMesh.name = name;
        if (ob == null) {
            ob = new GameObject(name);
            ob.AddComponent<MeshFilter>(); //网格
            ob.AddComponent<MeshRenderer>(); //网格渲染器  
        }

        ob.GetComponent<MeshFilter>().sharedMesh = walkMesh;
        GameObjectUtility.SetStaticEditorFlags(ob, StaticEditorFlags.NavigationStatic);
        GameObjectUtility.SetNavMeshArea(ob, WalkLayer);
        return ob;
    }

    /// <summary>
    /// 设置agent属性
    /// </summary>
    /// <param name="agentRadius"></param>
    private void SetAgentRadius(float agentRadius){
        SerializedObject settingsObject = new SerializedObject(UnityEditor.AI.NavMeshBuilder.navMeshSettingsObject);
        SerializedProperty agentRadiusSettings = settingsObject.FindProperty("m_BuildSettings.agentRadius");

        agentRadiusSettings.floatValue = agentRadius;

        settingsObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="type">0 阻挡 ；1行走；2安全</param>
    /// <returns></returns>
    static string MeshToString(UnityEngine.AI.NavMeshTriangulation mesh, int type){
        if (mesh.indices.Length < 1) {
            return "";
        }

        StringBuilder sb = new StringBuilder();
        sb.Append(type == 0 ? "\"blockTriangles\":[" : (type == 1 ? "\"pathTriangles\":[" : "\"safeTriangles\":["));
        for (int i = 0; i < mesh.indices.Length; i++) {
            sb.Append(mesh.indices[i]).Append(",");
        }

        sb.Length--;
        sb.Append("],");

        sb.Append(type == 0 ? "\"blockVertices\":[" : (type == 1 ? "\"pathVertices\":[" : "\"safeVertices\":["));
        for (int i = 0; i < mesh.vertices.Length; i++) {
            Vector3 v = mesh.vertices[i];
            if (type > 0 && v.y < 1) {
                Debug.LogWarning("寻路mesh坐标小于1" + v.y);
            }

            sb.Append("{\"x\":").Append(v.x).Append(",\"y\":").Append(type == 0 ? 0 : v.y).Append(",\"z\":").Append(v.z)
                .Append("},");
        }

        sb.Length--;
        sb.Append("]");
        return sb.ToString();
    }

    static void MeshToFile(string filename, string meshData){
        using (StreamWriter sw = new StreamWriter(filename)) {
            sw.Write(meshData);
        }
    }
}
#endif