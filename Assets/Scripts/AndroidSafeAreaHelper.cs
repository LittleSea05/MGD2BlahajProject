using UnityEngine;

public static class AndroidSafeAreaHelper
{
    // 查询Android系统实际的底部导航栏/手势条高度（单位：像素，跟Screen.height同单位）
    // 编辑器和非Android平台会直接返回0，不影响其他平台
    public static float GetBottomNavBarHeight()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow"))
            using (AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView"))
            using (AndroidJavaObject insets = decorView.Call<AndroidJavaObject>("getRootWindowInsets"))
            {
                if (insets == null) return 0f;
                int bottom = insets.Call<int>("getSystemWindowInsetBottom");
                return bottom;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"AndroidSafeAreaHelper: 查询导航栏高度失败, {e.Message}");
            return 0f;
        }
#else
        return 0f;
#endif
    }
}