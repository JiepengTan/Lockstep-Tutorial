#if UNITY_EDITOR
using UnityEngine;
using Lockstep.Math;

namespace UnityEditor {
    public static class EditorGUILayoutExt {
        public static LFloat FloatField( string label,LFloat value,params GUILayoutOption[] options){
            return EditorGUILayout.FloatField(label, value.ToFloat(),options).ToLFloat();
        }
        public static LVector2 Vector2Field( string label,LVector2 value,params GUILayoutOption[] options){
            return EditorGUILayout.Vector2Field(label, value.ToVector2(),options).ToLVector2();
        }
        public static LVector3 Vector3Field( string label,LVector3 value,params GUILayoutOption[] options){
            return EditorGUILayout.Vector3Field(label, value.ToVector3(),options).ToLVector3();
        }  
    }
}
#endif    