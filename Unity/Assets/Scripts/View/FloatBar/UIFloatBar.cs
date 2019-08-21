using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//NOTE! You should hava a Camera with "MainCamera" tag and a canvas with a Screen Space - Overlay mode to script works properly;
public class UIFloatBar : MonoBehaviour {
    [Header("UI ref")]
    public Image healthVolume;
    public Image backGround; //Health bar images, should be named as "Health" and "Background";
    public Text healthInfo;
    private RectTransform healInfoRectTrans;
    private RectTransform rectTransform;

    [Header("Prefab define")]
    public FloatBarConfig config ;
    public float yOffset = 2.55f;
    public float scale = 1;
    public Vector2 sizeOffsets;

    private Vector2 rawSizeDelta;
    private Vector2 healthInfoPosition;
    private Transform taragetTrans;
    private int _maxVal;
    private int _curVal;
    private float camDistance;
    private float delayTimestamp;
    private CanvasGroup canvasGroup;
    private Camera cam => FloatBarManager.cam;
    private Canvas canvas => FloatBarManager.canvas;


    public bool keepSize => config.keepSize;
    public bool IsDrawOffDistance => config.IsDrawOffDistance;
    public float drawDistance => config.drawDistance;
    public bool showHealthInfo => config.showHealthInfo;
    public HealthInfoAlignment healthInfoAlignment => config.healthInfoAlignment;
    public float healthInfoSize => config.healthInfoSize;
    
    void Awake(){
        config = FloatBarManager.config;
        if (healthVolume == null) {
            healthVolume = transform.Find("Health").GetComponent<Image>();
        }

        if (backGround == null) {
            backGround = transform.Find("Background").GetComponent<Image>();
        }

        if (healthInfo == null) {
            healthInfo = transform.Find("HealthInfo").GetComponent<Text>();
        }

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        healInfoRectTrans = healthInfo.rectTransform;

        healthInfo.resizeTextForBestFit = true;
        healInfoRectTrans.anchoredPosition = Vector2.zero;
        healthInfo.resizeTextMinSize = 1;
        healthInfo.resizeTextMaxSize = 500;

        healthInfoPosition = healthInfo.rectTransform.anchoredPosition;
        rawSizeDelta = rectTransform.sizeDelta;
        canvasGroup.alpha = config.fullAlpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        if (healthInfoAlignment == HealthInfoAlignment.Top)
            healInfoRectTrans.anchoredPosition = healthInfoPosition;
        else if (healthInfoAlignment == HealthInfoAlignment.Center)
            healInfoRectTrans.anchoredPosition = Vector2.zero;
        else
            healInfoRectTrans.anchoredPosition = -healthInfoPosition;
    }


    public void OnRecycle(){
        gameObject.SetActive(false);
    }

    public void OnUse(Transform tran, int val, int maxVal){
        taragetTrans = tran;
        var pos = tran.position;
        transform.position =FloatBarManager.cam.WorldToScreenPoint(new Vector3(pos.x, pos.y + yOffset, pos.z));
        gameObject.SetActive(true);
        UpdateHp(val, maxVal);
    }


    public void UpdateHp(int curVal, int maxVal){
        healthVolume.fillAmount = curVal / (maxVal * 1.0f);
        delayTimestamp = Time.time + config.onHit.duration;


        if (showHealthInfo)
            healthInfo.text = curVal + " / " + maxVal;
        else
            healthInfo.text = "";
        _curVal = curVal;
        _maxVal = maxVal;
        if (_curVal <= 0) _curVal = 0;
        if (_curVal > _maxVal) _maxVal = _curVal;
    }

    public void DoUpdate(float deltaTime){
        if (!rectTransform)
            return;

        if (taragetTrans == null) return;
        var pos = taragetTrans.position;
        var curTranPos = FloatBarManager.cam.WorldToScreenPoint(new Vector3(pos.x, pos.y + yOffset, pos.z));
        transform.position = Vector3.Lerp(transform.position, curTranPos, 0.3f);
        var camDistance = Vector3.Dot(taragetTrans.position - cam.transform.position, cam.transform.forward);
#if false
        if (
            //!OutDistance(camDistance) && 
            //IsVisible() &&
            true
            ) {
            if (_curVal <= 0) {
                if (config.nullFadeSpeed > 0) {
                    if (backGround.fillAmount <= 0)
                        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, config.nullAlpha,
                            config.nullFadeSpeed);
                }
                else
                    canvasGroup.alpha = config.nullAlpha;
            }
            else if (_curVal == _maxVal)
                canvasGroup.alpha = config.fullFadeSpeed > 0
                    ? Mathf.MoveTowards(canvasGroup.alpha, config.fullAlpha,
                        config.fullFadeSpeed)
                    : config.fullAlpha;
            else {
                if (delayTimestamp > Time.time)
                    canvasGroup.alpha = config.onHit.onHitAlpha;
                else
                    canvasGroup.alpha = config.onHit.fadeSpeed > 0
                        ? Mathf.MoveTowards(canvasGroup.alpha, config.defaultAlpha,
                            config.onHit.fadeSpeed)
                        : config.defaultAlpha;
            }
        }
        else
            canvasGroup.alpha = config.defaultFadeSpeed > 0
                ? Mathf.MoveTowards(canvasGroup.alpha, 0, config.defaultFadeSpeed)
                : 0;

#endif
        var dist = keepSize ? camDistance / scale : scale;

       // rectTransform.sizeDelta = new Vector2(rawSizeDelta.x / (dist - sizeOffsets.x / 100),
       //     rawSizeDelta.y / (dist - sizeOffsets.y / 100));
//
       // healInfoRectTrans.sizeDelta = new Vector2(rectTransform.sizeDelta.x * healthInfoSize / 10,
       //     rectTransform.sizeDelta.y * healthInfoSize / 10);
//
       // healthInfoPosition.y = rectTransform.sizeDelta.y +
       //                        (healInfoRectTrans.sizeDelta.y - rectTransform.sizeDelta.y) / 2;



        float maxDifference = 0.1F;
        if (backGround.fillAmount > healthVolume.fillAmount + maxDifference)
            backGround.fillAmount = healthVolume.fillAmount + maxDifference;
        if (backGround.fillAmount > healthVolume.fillAmount)
            backGround.fillAmount -= (1 / (_maxVal / 100.0f)) * deltaTime;
        else
            backGround.fillAmount = healthVolume.fillAmount;

    }


    bool IsVisible(){
        return canvas.pixelRect.Contains(rectTransform.position);
    }

    bool OutDistance(float camDistance){
        return IsDrawOffDistance == true && camDistance > drawDistance;
    }
}