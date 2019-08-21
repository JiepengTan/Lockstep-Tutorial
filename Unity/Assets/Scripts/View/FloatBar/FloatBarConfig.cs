using UnityEngine;

[CreateAssetMenu(menuName = "HealthBarConfig")]
[System.Serializable]
public class FloatBarConfig : ScriptableObject {
    public float defaultAlpha = 0.7F; //Default healthbar alpha (health is bigger then zero and not full);
    public float defaultFadeSpeed = 0.1F;
    public float fullAlpha = 1.0F; //Healthbar alpha when health is full;
    public float fullFadeSpeed = 0.1F;
    public float nullAlpha = 0.0F; //Healthbar alpha when health is zero or less;
    public float nullFadeSpeed = 0.1F;
    public OnHit onHit = new OnHit(); //On hit settings
    
    
    public bool keepSize = true;
    public bool IsDrawOffDistance;
    public float drawDistance = 10;
    public bool showHealthInfo;
    public HealthInfoAlignment healthInfoAlignment = HealthInfoAlignment.Center;
    public float healthInfoSize = 10;
}

public enum HealthInfoAlignment {
    Top,
    Center,
    Bottom
};

[System.Serializable]
public class OnHit {
    public float fadeSpeed = 0.1F; //Alpha state fade speed;
    public float onHitAlpha = 1.0F; //On hit alpha;
    public float duration = 1.0F; //Duration of alpha state;
}
