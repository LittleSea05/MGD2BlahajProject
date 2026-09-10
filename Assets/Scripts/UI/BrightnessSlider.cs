using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BrightnessSlider : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        if (BrightnessManager.Instance != null)
        {
            slider.SetValueWithoutNotify(BrightnessManager.Instance.GetBrightness());
        }

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        if (BrightnessManager.Instance != null)
        {
            BrightnessManager.Instance.SetBrightness(value);
        }
    }
}