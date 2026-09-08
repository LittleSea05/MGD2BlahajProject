using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeDisc : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [Header("Volume")]
    [Range(0,1)]
    public float volume = 1f;

    [Header("Rotation")]
    public float maxSpinSpeed = 120f;

    private bool dragging = false;

    public Toggle muteToggle;

    void Start()
    {

        muteToggle.onValueChanged.AddListener(OnMuteToggle);

    }

    void OnMuteToggle(bool isMuted)
    {
        if (isMuted)
        {
            volume = 0f;
        }
        else
        {
            volume = 1f;
        }
    }

    void Update()
    {

        if (!dragging && volume > 0)
        {
            transform.Rotate(0,0,-maxSpinSpeed * volume * Time.deltaTime);
        }

      
         AudioListener.volume = volume;

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

        transform.rotation = Quaternion.Euler(0,0,angle);



        float normalized =
            Mathf.InverseLerp(-180,180,angle);

        volume = normalized;

  
         AudioListener.volume = volume;
    }
}