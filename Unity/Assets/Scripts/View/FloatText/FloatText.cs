using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class FloatText : MonoBehaviour {
    public FloatTextManager mgr;

    public Color color {
        set => textDamage.color = value;
    }

    public Text textDamage;

    [Header("Debug")] [SerializeField] private float timer;
    [SerializeField] private RectTransform textRectTransform;

    public bool IsFinished => timer > mgr.slideTotalTime;
    public static Camera _worldCam;

    public static Camera WorldCam {
        get {
            if (_worldCam == null) {
                _worldCam = Camera.main;
            }

            return _worldCam;
        }
    }

    private void Awake(){
        textRectTransform = textDamage.rectTransform;
        textRectTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void DoUpdate(float deltaTime){
        timer += deltaTime;
        transform.position = WorldCam.WorldToScreenPoint(bornPos);
        // Destroy self
        var pos = textRectTransform.anchoredPosition;
        float percent = Mathf.Clamp01(timer / mgr.slideTotalTime);
        SetAlpha(mgr.GetAlpha(percent));
        textRectTransform.anchoredPosition = new Vector2(mgr.GetXOffset(percent), pos.y + mgr.GetYOffset(percent));
        textRectTransform.localScale = Vector3.one * mgr.GetScale(percent);
    }

    void SetAlpha(float alpha){
        var color = textDamage.color;
        color.a = alpha;
        textDamage.color = color;
    }

    public void OnRecycle(){
        gameObject.SetActive(false);
        textRectTransform.anchoredPosition = Vector2.zero;
        textDamage.CrossFadeAlpha(1, 0, true);
    }

    private float rawXAnchoPos;
    private Vector3 bornPos;
    public void OnUse(Vector3 pos, string text){
        bornPos = pos;
        textDamage.transform.localScale = Vector3.one;
        transform.position = WorldCam.WorldToScreenPoint(pos);
        rawXAnchoPos = 0;
        textDamage.text = text;
        gameObject.SetActive(true);
        timer = 0;
    }
}