using UnityEngine;
using DG.Tweening;

public class PlasticBagFloat : MonoBehaviour
{

    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private float floatDuration = 2.5f;

   
    [SerializeField] private bool enableSway = true;
    [SerializeField] private float swayAngle = 5f;
    [SerializeField] private float swayDuration = 3f;


    [SerializeField] private bool randomizeStart = true;

    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float disappearDuration = 0.4f;

    private Vector3 startPos;
    private bool hasHit = false;

    void Start()
    {
        startPos = transform.position;

        if (randomizeStart)
        {
            floatDuration += Random.Range(-0.4f, 0.4f);
            swayDuration += Random.Range(-0.5f, 0.5f);
        }

        PlayFloatAnimation();

        if (enableSway)
            PlaySwayAnimation();
    }

    private void PlayFloatAnimation()
    {
        transform.DOMoveY(startPos.y + floatHeight, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(randomizeStart ? Random.Range(0f, 1f) : 0f);
    }

    private void PlaySwayAnimation()
    {
        transform.DORotate(new Vector3(0, 0, swayAngle), swayDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(randomizeStart ? Random.Range(0f, 1f) : 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (!other.CompareTag(playerTag)) return;

        hasHit = true;

        if (HealthSlider.Instance != null)
        {
            HealthSlider.Instance.AddHealth(-damageAmount);
        }

        PlayDisappearAndDestroy();
    }

    private void PlayDisappearAndDestroy()
    {
        transform.DOKill();

        Sequence disappearSeq = DOTween.Sequence();
        disappearSeq.Join(transform.DOScale(Vector3.zero, disappearDuration).SetEase(Ease.InBack));
        disappearSeq.Join(transform.DOMoveY(transform.position.y + 0.3f, disappearDuration));

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null && rend.material.HasProperty("_Color"))
        {
            disappearSeq.Join(rend.material.DOFade(0f, disappearDuration));
        }

        disappearSeq.OnComplete(() => Destroy(gameObject));
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}