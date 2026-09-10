using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    private const string BRIGHTNESS_KEY = "ScreenBrightness";

    public Image overlayImage;

    [Range(0f, 1f)]
    public float maxOverlayAlpha = 0.85f;

    private float currentBrightness = 1f; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
        ApplyBrightness();
    }

    public float GetBrightness()
    {
        return currentBrightness;
    }

    public void SetBrightness(float value)
    {
        currentBrightness = Mathf.Clamp01(value);
        ApplyBrightness();

        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, currentBrightness);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness()
    {
        if (overlayImage == null) return;

        float alpha = (1f - currentBrightness) * maxOverlayAlpha;
        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}