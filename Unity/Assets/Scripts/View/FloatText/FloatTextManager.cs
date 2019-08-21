using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloatTextManager : MonoBehaviour {
    [Header("Curves")] public AnimationCurve curveAlpha = null;
    [HideInInspector] public AnimationCurve curveScale = null;
    [HideInInspector] public AnimationCurve curveX = null;
    [HideInInspector] public AnimationCurve curveY = null;

    [HideInInspector] public Color color;
    [Header("Times")] public float slideTotalTime;
    public float yOffset = 5;
    public float xOffset = 5;

    [Header("Pools")] public GameObject prefab;
    public int maxPoolCount = 50;

    private static FloatTextManager _Instance;
    [SerializeField] private Canvas canvas;
    private List<FloatText> runingTexts = new List<FloatText>();
    private Queue<FloatText> pools = new Queue<FloatText>();

    public float GetAlpha(float percent){
        return curveAlpha.Evaluate(percent);
    }

    public float GetXOffset(float percent){
        var val = curveX.Evaluate(percent) * xOffset;
        return val;
    }

    public float GetYOffset(float percent){
        return curveY.Evaluate(percent) * yOffset;
    }

    public float GetScale(float percent){
        return curveScale.Evaluate(percent);
    }

    private void Update(){
        var deltaTime = Time.deltaTime;
        foreach (var text in runingTexts) {
            text.DoUpdate(deltaTime);
        }

        for (int i = runingTexts.Count - 1; i >= 0; --i) {
            var text = runingTexts[i];
            if (text.IsFinished) {
                runingTexts.RemoveAt(i);
                DestroyText(text);
            }
        }
    }

    public FloatTextConfig config;

    private void Start(){
        _Instance = this;
        if (prefab == null) {
            prefab = (GameObject) Resources.Load("Prefabs/FloatingDamage");
        }

        if (config == null) {
            config = Resources.Load<FloatTextConfig>("Config/FloatDamageConfig");
        }

        curveAlpha = config.curveAlpha;
        curveScale = config.curveScale;
        curveX = config.curveX;
        curveY = config.curveY;
        color = config.color;
        slideTotalTime = config.slideTotalTime;
        yOffset = config.yOffset;
        xOffset = config.xOffset;
    }

    private void DestroyText(FloatText text){
        if (pools.Count > _Instance.maxPoolCount) {
            GameObject.Destroy(text);
            return;
        }

        text.OnRecycle();
        pools.Enqueue(text);
    }

    private FloatText GetOrCreateText(){
        if (pools.Count > 0) {
            return pools.Dequeue();
        }

        var obj = Instantiate(prefab, canvas.transform, false);
        var text = obj.GetComponent<FloatText>();
        return text;
    }

    public static void CreateFloatText(Vector3 pos, int damage){
        _Instance._CreateFloatText(pos, damage > 0 ? "+" + damage.ToString() : damage.ToString(),
            damage > 0 ? _Instance.config.healColor : _Instance.config.atkColor);
    }

    public static void CreateFloatText(Vector3 pos, string text, Color color){
        _Instance._CreateFloatText(pos, text, color);
    }

    private void _CreateFloatText(Vector3 pos, string text, Color color){
        var fd = GetOrCreateText();
        fd.mgr = this;
        fd.color = color;
        runingTexts.Add(fd);
        fd.OnUse(pos, text);
    }
}