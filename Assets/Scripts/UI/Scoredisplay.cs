using UnityEngine;
using TMPro;

/// <summary>
/// 唯一负责显示"分数"和"金币"的脚本，其他脚本(HealthSlider、ShopManager等)
/// 都不直接碰Text，只调用这个脚本提供的方法。
///
/// 三个字段都是可选的，哪个场景需要用哪个就拖进去，不需要的留空即可：
/// - liveScoreText：游戏进行中实时显示本局分数（挂在游戏场景的Canvas上）
/// - finalScoreText：死亡/结算面板上显示"这局打了多少分"
/// - coinsText：死亡面板 或 商店场景 显示"总共有多少金币"，两边用的是同一个数据源，
///   数字必定一致
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [Header("游戏进行中：实时分数（可选）")]
    public TMP_Text liveScoreText;

    [Header("死亡/结算面板：本局最终分数（可选）")]
    public TMP_Text finalScoreText;

    [Header("死亡面板 或 商店：总金币（可选）")]
    public TMP_Text coinsText;

    void Start()
    {
        if (liveScoreText != null && ScoreManager.Instance != null)
        {
            UpdateLiveScore(ScoreManager.Instance.runScore);
            ScoreManager.Instance.OnScoreChanged += UpdateLiveScore;
        }

        // 只要coinsText被拖了值，进场景就先刷新一次（商店场景靠这个显示金币）
        RefreshCoins();
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateLiveScore;
        }
    }

    void UpdateLiveScore(int score)
    {
        if (liveScoreText != null)
        {
            liveScoreText.text = $"Score: {score}";
        }
    }

    /// <summary>刷新金币显示。买完升级之后也可以手动调用它来更新数字。</summary>
    public void RefreshCoins()
    {
        if (coinsText != null && ScoreManager.Instance != null)
        {
            coinsText.text = $"Coins: {ScoreManager.Instance.TotalCoins}";
        }
    }

    /// <summary>
    /// 游戏结束时调用一次：显示本局最终分数 + 结算后的总金币。
    /// finalScore必须在ScoreManager.EndRun()把runScore清零之前先记下来传进来。
    /// </summary>
    public void ShowGameOverResults(int finalScore)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {finalScore}";
        }
        RefreshCoins();
    }
}