using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SFXSlider : MonoBehaviour
{
    private Slider slider;

    void Awake() => slider = GetComponent<Slider>();

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            slider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
        }
        slider.onValueChanged.AddListener(OnChanged);
    }

    void OnDestroy() => slider.onValueChanged.RemoveListener(OnChanged);

    void OnChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }
}