using System;
using UnityEngine;
using System.Collections;
using Lockstep.PathFinding;

[Serializable]
public class NavMeshTool : MonoBehaviour {
    public int mapId;
    public GameObject mapRoot;
    public TriangleData data = new TriangleData();
}
