using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeZone : MonoBehaviour
{
    public float extraTopPadding = 0f;

    RectTransform rect;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);
    int lastScreenWidth;
    int lastScreenHeight;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        lastSafeArea = safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float minY = safeArea.yMin;
        float maxY = safeArea.yMax - extraTopPadding;

        Vector2 anchorMin = new Vector2(
            safeArea.xMin / Screen.width,
            minY / Screen.height
        );

        Vector2 anchorMax = new Vector2(
            safeArea.xMax / Screen.width,
            maxY / Screen.height
        );

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }
}