using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityView))]
public class EditorEnemyView : Editor {
    private EntityView owner;
    public LVector3 force;
    public LFloat resetYSpd;
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = target as EntityView;
        force = EditorGUILayoutExt.Vector3Field("force", force);
        if (GUILayout.Button("AddImpulse")) {
            owner.entity.rigidbody.AddImpulse(force);
        }

        resetYSpd = EditorGUILayoutExt.FloatField("resetYSpd", resetYSpd);
        if (GUILayout.Button("ResetSpeed")) {
            owner.entity.rigidbody.ResetSpeed(resetYSpd);
        }
    }
}