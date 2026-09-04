using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeZone : MonoBehaviour
{
    RectTransform rect;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // 处理旋转屏幕/切换分辨率时的变化
        if (Screen.safeArea != lastSafeArea)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }
}