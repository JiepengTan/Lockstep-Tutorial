#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LFloat))]
public class EditorLFloat : UnityEditor.PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        var xProperty = property.FindPropertyRelative("_val");
        float LabelWidth = EditorGUIUtility.labelWidth - EditorLVectorDrawTool.LableWidthOffset;
        float lableWid = EditorLVectorDrawTool.LableWid;
        var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);
        float filedWid = (position.width - LabelWidth);
        float initX = position.x + LabelWidth;
        var valRect = new Rect(initX, position.y, filedWid, position.height);
        var fVal = EditorGUI.FloatField(valRect, xProperty.intValue * 1.0f / LFloat.Precision);
        xProperty.intValue = (int) (fVal * LFloat.Precision);
    }
}
#endif