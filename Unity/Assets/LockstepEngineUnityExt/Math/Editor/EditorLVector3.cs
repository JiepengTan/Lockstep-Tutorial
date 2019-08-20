#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(LVector3))]
public class EditorLVector3 : UnityEditor.PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        var xProperty = property.FindPropertyRelative("_x");
        var yProperty = property.FindPropertyRelative("_y");
        var zProperty = property.FindPropertyRelative("_z");
        float LabelWidth = EditorGUIUtility.labelWidth - EditorLVectorDrawTool.LableWidthOffset;
        float lableWid = EditorLVectorDrawTool.LableWid;

        var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);
        float filedWid = (position.width - LabelWidth) / 3 - lableWid;
        float initX = position.x + LabelWidth;
        float offset = 0;
        EditorLVectorDrawTool.DrawField(position, initX, ref offset, lableWid, filedWid, xProperty, new GUIContent("x:"));
        EditorLVectorDrawTool.DrawField(position, initX, ref offset, lableWid, filedWid, yProperty, new GUIContent("y:"));
        EditorLVectorDrawTool.DrawField(position, initX, ref offset, lableWid, filedWid, zProperty, new GUIContent("z:"));
    }
}
#endif