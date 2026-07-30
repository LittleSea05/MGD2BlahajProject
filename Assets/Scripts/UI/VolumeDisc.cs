using UnityEngine;
using UnityEngine.EventSystems;

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

    void Update()
    {
        // 没有拖曳的时候，自己慢慢旋转
        if (!dragging && volume > 0)
        {
            transform.Rotate(0,0,-maxSpinSpeed * volume * Time.deltaTime);
        }

        // ===== Audio =====
         AudioListener.volume = volume;
        //
        // 或者
        // audioSource.volume = volume;
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

        //------------------------------------------------
        // 把角度转换成音量
        //------------------------------------------------

        float normalized =
            Mathf.InverseLerp(-180,180,angle);

        volume = normalized;

        // ===== Audio =====
         AudioListener.volume = volume;
    }
}