using UnityEngine;
using DG.Tweening;

public class HealthPotion : MonoBehaviour
{

    [SerializeField] private float rotateDuration = 3f;
    [SerializeField] private Vector3 rotateAxis = Vector3.up;


    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float disappearDuration = 0.4f;

    private bool hasCollected = false;

    void Start()
    {
        PlayRotateAnimation();
    }

    private void PlayRotateAnimation()
    {
        transform.DORotate(rotateAxis * 360f, rotateDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollected) return;
        if (!other.CompareTag(playerTag)) return;

        hasCollected = true;

        if (HealthSlider.Instance != null)
        {
            HealthSlider.Instance.AddHealth(HealthSlider.Instance.maxHealth);
        }

        PlayCollectAndDestroy();
    }

    private void PlayCollectAndDestroy()
    {
        transform.DOKill();

        Sequence collectSeq = DOTween.Sequence();
        collectSeq.Join(transform.DOScale(Vector3.zero, disappearDuration).SetEase(Ease.InBack));
        collectSeq.Join(transform.DOMoveY(transform.position.y + 0.5f, disappearDuration));

        collectSeq.OnComplete(() => Destroy(gameObject));
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}