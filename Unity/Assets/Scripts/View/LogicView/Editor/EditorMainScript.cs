using Lockstep.Math;
using Lockstep.Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainScript))]
public class EditorMainScript : Editor {
    private MainScript owner;
    public int rollbackTickCount = 90;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = target as MainScript;
        rollbackTickCount = EditorGUILayout.IntField("RollbackTickCount", rollbackTickCount);
        if (GUILayout.Button("Rollback")) {
            owner.GetService<ICommonStateService>().IsPause = true;
            var world = ((owner.GetService<ISimulatorService>()) as SimulatorService).World;
            world.RollbackTo(world.Tick - rollbackTickCount,0,false);
        }
    }
}