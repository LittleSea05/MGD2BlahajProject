using UnityEngine;
using DG.Tweening;

public class UIFloat : MonoBehaviour
{
    [Header("Float")]
    public float floatDistance = 20f;
    public float floatDuration = 2.5f;

    [Header("Rotation")]
    public float rotateAngle = 3f;
    public float rotateDuration = 3f;

    private RectTransform rect;
    private Vector3 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.localPosition;

        // 给每个UI一点随机相位，不会全部一起动
        float delay = Random.Range(0f, floatDuration);

        // 上下漂浮
        rect.DOLocalMoveY(startPos.y + floatDistance, floatDuration)
            .From(startPos.y - floatDistance)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(delay);

        // 左右轻轻摇摆
        rect.DOLocalRotate(new Vector3(0, 0, rotateAngle), rotateDuration)
            .From(new Vector3(0, 0, -rotateAngle))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(delay);
    }
}