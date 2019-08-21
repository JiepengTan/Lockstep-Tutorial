using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = Lockstep.Logging.Debug;

public class FloatBarManager : MonoBehaviour {
    private static FloatBarManager _Instance;
    private List<Transform> healthBars = new List<Transform>(); //List of helthbars;
    private Transform cameraTransform; //Main camera transform
    [Header("Pools")] public GameObject prefab;
    public int maxPoolCount = 50;

    public Canvas _canvas;
    public static Canvas canvas => _Instance._canvas;
    private List<UIFloatBar> allFloatElem = new List<UIFloatBar>();
    private Queue<UIFloatBar> pools = new Queue<UIFloatBar>();
    public FloatBarConfig _config;
    public static FloatBarConfig config;

    public static Camera cam;
    private Transform poolTrans;
    private Transform showTrans;

    void Awake(){
        cam = Camera.main;
        cameraTransform = cam.transform;

        for (int i = 0; i < transform.childCount; i++)
            healthBars.Add(transform.GetChild(i));

        _Instance = this;
        if (prefab == null) {
            prefab = (GameObject) Resources.Load("Prefabs/FloatingDamage");
        }

        if (_config == null) {
            _config = (FloatBarConfig) Resources.Load("FloatBarConfig");
        }

        config = _config;
        poolTrans = _CreateTrans("HealthbarPool");
        showTrans = _CreateTrans("HealthbarRoot");
    }

    Transform _CreateTrans(string name){
        var tran = new GameObject(name).transform;
        tran.SetParent(canvas.transform, false);
        tran.localPosition = Vector3.zero;
        tran.localRotation = Quaternion.identity;
        tran.localScale = Vector3.one;
        return tran;
    }

    void Update(){
        var deltaTime = Time.deltaTime;
        healthBars.Sort(DistanceCompare);

        for (int i = 0; i < healthBars.Count; i++)
            healthBars[i].SetSiblingIndex(healthBars.Count - (i + 1));
        foreach (var val in allFloatElem) {
            val.DoUpdate(deltaTime);
        }
    }

    private int DistanceCompare(Transform a, Transform b){
        return Mathf.Abs((WorldPos(a.position) - cameraTransform.position).sqrMagnitude)
            .CompareTo(Mathf.Abs((WorldPos(b.position) - cameraTransform.position).sqrMagnitude));
    }

    private Vector3 WorldPos(Vector3 pos){
        return cam.ScreenToWorldPoint(pos);
    }

    private void _DestroyText(UIFloatBar text){
        if (pools.Count > _Instance.maxPoolCount) {
            GameObject.Destroy(text);
            return;
        }

        text.OnRecycle();
        text.transform.SetParent(poolTrans, false);
        pools.Enqueue(text);
    }

    private UIFloatBar GetOrCreateText(){
        if (pools.Count > 0) {
            var go = pools.Dequeue();
            go.transform.SetParent(showTrans, false);
            return go;
        }

        var obj = Instantiate(prefab, showTrans, false);
        return obj.GetComponent<UIFloatBar>();
    }

    public static void DestroyText(UIFloatBar trans){
        _Instance?._DestroyText(trans);
    }

    public static UIFloatBar CreateFloatBar(Transform trans, int val, int maxVal){
        return _Instance?._CreateFloatBar(trans, val, maxVal);
    }

    private UIFloatBar _CreateFloatBar(Transform trans, int val, int maxVal){
        var fd = GetOrCreateText();
        allFloatElem.Add(fd);
        fd.OnUse(trans, val, maxVal);
        return fd;
    }
}