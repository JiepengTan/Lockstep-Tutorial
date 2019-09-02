using System.Collections;
using System.Collections.Generic;
using Lockstep.Game;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

internal class LayerMatrixGUI { }

[CustomEditor(typeof(CollisionConfig))]
public class EditorCollisionSystem : Editor {
    private CollisionConfig owner;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        //owner = target as CollisionConfig;
        //DoGUI("LayerCollisionMatrix", ref owner.isShow, ref owner.scrollPos, owner.GetColliderPair,
        //    owner.SetColliderPair);
    }


    public delegate bool GetValueFunc(int layerA, int layerB);

    public delegate void SetValueFunc(int layerA, int layerB, bool val);

    public string LayerToName(int idx){
        if (idx < 0 || idx >= owner.ColliderLayerNames.Length)
            return "";
        return owner.ColliderLayerNames[idx];
    }

    public void DoGUI(string title, ref bool show, ref Vector2 scrollPos, GetValueFunc getValue, SetValueFunc setValue){
        const int kMaxLayers = 32;
        const int checkboxSize = 16;
        int labelSize = 100;
        const int indent = 30;

        int numLayers = 0;
        for (int i = 0; i < kMaxLayers; i++)
            if (LayerToName(i) != "")
                numLayers++;

        // find the longest label
        for (int i = 0; i < kMaxLayers; i++) {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(LayerToName(i)));
            if (labelSize < textDimensions.x)
                labelSize = (int) textDimensions.x;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(0);
        show = EditorGUILayout.Foldout(show, title, true);
        GUILayout.EndHorizontal();
        if (show) {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(labelSize + 20),
                GUILayout.MaxHeight(labelSize + (numLayers + 1) * checkboxSize));
            int y = 0;
            int lx = 0;
            Rect rr = GUILayoutUtility.GetRect(indent + checkboxSize * numLayers + labelSize, checkboxSize);
            for (int i = 0; i < kMaxLayers; i++) {
                if (LayerToName(i) != "") {
                    GUI.Label(new Rect(labelSize + indent + rr.x + lx * checkboxSize, rr.y, checkboxSize, checkboxSize),
                        (numLayers - i-1).ToString(), "RightLabel");
                    lx++;
                }
            }

            GUI.matrix = Matrix4x4.identity;
            y = 0;
            for (int i = 0; i < kMaxLayers; i++) {
                if (LayerToName(i) != "") {
                    int x = 0;
                    Rect r = GUILayoutUtility.GetRect(indent + checkboxSize * numLayers + labelSize, checkboxSize);
                    GUI.Label(new Rect(r.x + indent, r.y, labelSize, checkboxSize), i + " " + LayerToName(i),
                        "RightLabel");
                    for (int j = kMaxLayers-1; j >=0; j--) {
                        if (LayerToName(j) != "") {
                            if (x < numLayers - y) {
                                GUIContent tooltip = new GUIContent("", LayerToName(i) + "/" + LayerToName(j));
                                bool val = getValue(i, j);
                                bool toggle =
                                    GUI.Toggle(
                                        new Rect(labelSize + indent + r.x + x * checkboxSize, r.y, checkboxSize,
                                            checkboxSize), val, tooltip);
                                if (toggle != val)
                                    setValue(i, j, toggle);
                            }

                            x++;
                        }
                    }

                    y++;
                }
            }

            GUILayout.EndScrollView();
        }
    }
}