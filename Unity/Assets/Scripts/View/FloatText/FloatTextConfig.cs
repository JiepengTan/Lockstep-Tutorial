using UnityEngine;

[CreateAssetMenu(menuName = "FloatTextConfig")]
public class FloatTextConfig : ScriptableObject {
    [Header("Curves")] public AnimationCurve curveAlpha = null;
    public AnimationCurve curveScale = null;
    public AnimationCurve curveX = null;
    public AnimationCurve curveY = null;

    public Color color;
    [Header("Times")] public float slideTotalTime;
    public float yOffset = 5;
    public float xOffset = 5;
    public Color atkColor;
    public Color healColor;
}