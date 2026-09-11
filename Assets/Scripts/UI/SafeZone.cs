using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeZone : MonoBehaviour
{
    [Header("保险边距（找不到精确导航栏高度时的备用值）")]
    public float fallbackBottomPadding = 40f;
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
        // 处理旋转屏幕/切换分辨率时的变化
        if (Screen.safeArea != lastSafeArea || Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        // 先尝试用Android原生API查询真实的导航栏高度，查不到就用备用固定值
        float navBarHeight = AndroidSafeAreaHelper.GetBottomNavBarHeight();
        float bottomPadding = navBarHeight > 0f ? navBarHeight : fallbackBottomPadding;

        float minY = safeArea.yMin + bottomPadding;
        float maxY = safeArea.yMax - extraTopPadding;

        Vector2 anchorMin = new Vector2(safeArea.xMin, minY);
        Vector2 anchorMax = new Vector2(safeArea.xMax, maxY);

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }
}