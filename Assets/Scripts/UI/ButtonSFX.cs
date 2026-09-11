using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour
{
    public AudioClip clickSfx;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        button.onClick.AddListener(PlayClickSfx);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(PlayClickSfx);
    }

    void PlayClickSfx()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlayButtonClickSFX(clickSfx);
    }
}