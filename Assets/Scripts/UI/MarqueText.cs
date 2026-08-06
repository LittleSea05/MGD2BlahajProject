using UnityEngine;
using DG.Tweening;

public class MarqueText : MonoBehaviour
{
    public RectTransform textA;
    public RectTransform textB;

    public float speed = 40f;      // 每秒移动多少像素
    public float spacing = 400f;   // 两个 Text 之间的距离
    public float resetY = 400f;    // 超过这个位置就重置
    public float startY = -400f;   // 重置到这里

    void Start()
    {
        textA.anchoredPosition = new Vector2(textA.anchoredPosition.x, startY);
        textB.anchoredPosition = new Vector2(textB.anchoredPosition.x, startY - spacing);
    }

    void Update()
    {
        MoveText(textA);
        MoveText(textB);
    }

    void MoveText(RectTransform text)
    {
        text.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (text.anchoredPosition.y >= resetY)
        {
            text.anchoredPosition = new Vector2(
                text.anchoredPosition.x,
                startY
            );
        }
    }
}