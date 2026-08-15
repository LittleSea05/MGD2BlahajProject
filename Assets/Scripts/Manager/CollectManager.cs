using UnityEngine;
using System.Collections;

public class CollectManager : MonoBehaviour
{
    public static CollectManager Instance;
    public CanvasGroup collectCanvasGroup;

    public float fadeInTime=0.5f;
    public float fadeOutTime=0.5f;
    public float showTime=1.0f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        Instance=this;
        collectCanvasGroup.alpha=0;
    }

    public void showPickUp()
    {
        if(fadeCoroutine!=null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine=StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade in
        float t=0f;
        while(t<fadeInTime)
        {
            t+=Time.deltaTime;
            collectCanvasGroup.alpha=Mathf.Clamp01(t/fadeInTime);
            yield return null;
        }
        collectCanvasGroup.alpha=1;

        // Wait for show time
        yield return new WaitForSeconds(showTime);

        t=0f;
        while(t<fadeOutTime)
        {
            t+=Time.deltaTime;
            collectCanvasGroup.alpha=Mathf.Clamp01(1-t/fadeOutTime);
            yield return null;
        }
        collectCanvasGroup.alpha=0;
    }



}
