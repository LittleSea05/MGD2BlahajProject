using UnityEngine;
using System.Collections;

public class LockStartButton : MonoBehaviour
{
    public static LockStartButton Instance;

    public CanvasGroup warningGroup;  
    public float fadeInTime = 0.3f;
    public float showTime = 1f;
    public float fadeOutTime = 0.5f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        Instance = this;
        warningGroup.alpha = 0f;
    }

    public void ShowWarning()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            warningGroup.alpha = Mathf.Clamp01(t / fadeInTime);
            yield return null;
        }
        warningGroup.alpha = 1f;

        yield return new WaitForSeconds(showTime);

        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            warningGroup.alpha = 1f - Mathf.Clamp01(t / fadeOutTime);
            yield return null;
        }
        warningGroup.alpha = 0f;
    }
}
