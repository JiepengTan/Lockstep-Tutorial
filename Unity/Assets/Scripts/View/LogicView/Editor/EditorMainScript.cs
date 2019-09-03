using Lockstep.Math;
using Lockstep.Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainScript))]
public class EditorMainScript : Editor {
    private MainScript owner;
    public int rollbackTickCount = 3;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = target as MainScript;
        if(!owner.HasInit ) return;
        var world = ((owner.GetService<ISimulatorService>()) as SimulatorService)?.World;
        EditorGUILayout.LabelField("CurTick " + world.Tick);
        rollbackTickCount = EditorGUILayout.IntField("RollbackTickCount", rollbackTickCount);
        if (GUILayout.Button("Rollback")) {
            owner.GetService<ICommonStateService>().IsPause = true;
            world.RollbackTo(world.Tick - rollbackTickCount,0,false);
        }
        if (GUILayout.Button("Resume")) {
            owner.GetService<ICommonStateService>().IsPause = false;
        }
    }
}