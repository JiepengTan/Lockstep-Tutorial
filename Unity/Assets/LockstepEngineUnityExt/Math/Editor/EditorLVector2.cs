#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LVector2))]
public class EditorLVector2 : UnityEditor.PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        var xProperty = property.FindPropertyRelative("_x");
        var yProperty = property.FindPropertyRelative("_y");
        float LabelWidth = EditorGUIUtility.labelWidth - EditorLVectorDrawTool.LableWidthOffset;
        float lableWid = EditorLVectorDrawTool.LableWid;

        var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);
        float filedWid = (position.width - LabelWidth) / 2 - lableWid;
        float initX = position.x + LabelWidth;
        float offset = 0;
        EditorLVectorDrawTool.DrawField(position, initX, ref offset, lableWid, filedWid, xProperty, new GUIContent("x:"));
        EditorLVectorDrawTool.DrawField(position, initX, ref offset, lableWid, filedWid, yProperty, new GUIContent("y:"));
    }
}
#endif