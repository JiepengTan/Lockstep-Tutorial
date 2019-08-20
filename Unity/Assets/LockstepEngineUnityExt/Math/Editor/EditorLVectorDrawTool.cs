#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
using UnityEditor;

public static class EditorLVectorDrawTool {
    public const float LableWidthOffset = 45;
    public const float LableWid = 20;

    public static void DrawField(Rect position, float initX, ref float offset, float lableWid, float filedWid,
        SerializedProperty property, GUIContent label){
        var lableRect = new Rect(initX + offset, position.y, 70, position.height);
        EditorGUI.LabelField(lableRect, label.text);
        var valRect = new Rect(initX + offset + lableWid, position.y, filedWid, position.height);
        var fVal = EditorGUI.FloatField(valRect, property.intValue * 1.0f / LFloat.Precision);
        property.intValue = (int) (fVal * LFloat.Precision);
        offset += filedWid + lableWid;
    }
}
#endif