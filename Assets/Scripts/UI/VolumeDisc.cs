using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class VolumeDisc : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [Header("Volume")]
    [Range(0, 1)]
    public float volume = 1f;

    [Header("Rotation")]
    public float maxSpinSpeed = 120f;

    private bool dragging = false;

    public Toggle muteToggle;

    [Header("UI tell volume value")]
    public TMP_Text volumeText;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            volume = AudioManager.Instance.GetMusicVolume();
        }

        if (muteToggle != null)
        {
            muteToggle.SetIsOnWithoutNotify(volume <= 0f);
            muteToggle.onValueChanged.AddListener(OnMuteToggle);
        }

        UpdateVolumeText();
    }

    void OnMuteToggle(bool isMuted)
    {
        volume = isMuted ? 0f : 1f;

        ApplyVolume();
        UpdateVolumeText();
    }

    void Update()
    {
        if (!dragging && volume > 0)
        {
            transform.Rotate(0, 0, -maxSpinSpeed * volume * Time.deltaTime);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dir =
            eventData.position -
            RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera,
                transform.position);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        float normalized = Mathf.InverseLerp(-180, 180, angle);
        volume = normalized;

        ApplyVolume();
        UpdateVolumeText();
    }

    void ApplyVolume()
    {
        AudioManager.Instance?.SetMusicVolume(volume);
    }

    void UpdateVolumeText()
    {
        if (volumeText == null) return;

        int percent = Mathf.RoundToInt(volume * 100f);
        volumeText.text = $"Volume: {percent}%";
    }
}