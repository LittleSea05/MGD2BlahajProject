using UnityEngine;

/// <summary>
/// 挂在每一条NPC鱼(可被玩家吃掉的鱼)身上。
/// 存放这条鱼的"体型大小"和被吃掉后的奖励数值。
/// 体型大小(fishSize)用来跟玩家的当前体型比较，决定谁吃谁。
/// </summary>
[RequireComponent(typeof(Collider))]
public class Fish : MonoBehaviour
{
    [Header("Size")]
    public float fishSize = 1f;
    public int scoreValue = 10;

    [Tooltip("Health++")]
    public float growthOnEat = 0.05f;

    /// <summary>
    /// 被玩家吃掉时调用。播放特效、通知管理器加分，然后销毁自己。
    /// 实际的"能不能吃"判定放在PlayerControl里做，这里只负责"被吃掉之后发生什么"。
    /// </summary>
    public void GetEaten()
    {

        // 如果你有分数管理器，可以在这里调用，例如：
        // ScoreManager.Instance.AddScore(scoreValue);

        Destroy(gameObject);
    }
}