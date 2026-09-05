using UnityEngine;

/// <summary>
/// 挂在每一条NPC鱼(可被玩家吃掉的鱼)身上。
/// 存放这条鱼的"体型大小"和被吃掉后的奖励数值。
/// 体型大小(fishSize)用来跟玩家的当前体型比较，决定谁吃谁。
/// </summary>
[RequireComponent(typeof(Collider))]
public class Fish : MonoBehaviour
{
    [Header("体型 / 判定")]
    [Tooltip("这条鱼的体型大小，用来跟玩家currentSize比较。数值越大鱼越大越难吃。")]
    public float fishSize = 1f;

    [Header("奖励")]
    [Tooltip("被玩家吃掉后获得的分数")]
    public int scoreValue = 10;

    [Tooltip("被吃掉后玩家体型增加多少（可选，做成长系统用）")]
    public float growthOnEat = 0.05f;

    [Tooltip("被吃掉后玩家恢复多少生命值")]
    public float healthRestoreOnEat = 5f;

    [Header("特效（可选）")]
    public GameObject eatEffectPrefab;

    /// <summary>
    /// 被玩家吃掉时调用。播放特效、通知管理器加分，然后销毁自己。
    /// 实际的"能不能吃"判定放在PlayerControl里做，这里只负责"被吃掉之后发生什么"。
    /// </summary>
    public void GetEaten()
    {
        if (eatEffectPrefab != null)
        {
            Instantiate(eatEffectPrefab, transform.position, Quaternion.identity);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        if (HealthSlider.Instance != null)
        {
            HealthSlider.Instance.AddHealth(healthRestoreOnEat);
        }

        Destroy(gameObject);
    }
}