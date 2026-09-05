using UnityEngine;
using System;

/// <summary>
/// 管理两种数字：
/// 1. runScore —— 这一局吃鱼获得的分数，游戏结束时会被"兑换"成金币
/// 2. TotalCoins —— 跨局保留、可以在商城里花的金币，存在PlayerPrefs里
///
/// 用法：
/// - 玩家吃鱼时调用 ScoreManager.Instance.AddScore(fish.scoreValue)
/// - 游戏结束（比如玩家死亡/回到主菜单）时调用 ScoreManager.Instance.EndRun()
///   这时候runScore会被加进TotalCoins并存档，同时runScore清零
/// - 商城场景里用 ScoreManager.Instance.TotalCoins 显示/扣除金币
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const string TOTAL_COINS_KEY = "TotalCoins";

    [Tooltip("本局当前分数，实时显示在UI上用")]
    public int runScore = 0;

    /// <summary>每次runScore变化时触发，参数是最新的runScore。UI脚本订阅它来实时刷新显示。</summary>
    public event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>吃到鱼、捡到宝箱等时调用，增加本局分数</summary>
    public void AddScore(int amount)
    {
        runScore += amount;
        OnScoreChanged?.Invoke(runScore);
    }

    /// <summary>当前保留的总金币（还没结算本局分数）</summary>
    public int TotalCoins
    {
        get => PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        private set
        {
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 游戏结束时调用：把本局分数结算进总金币并存档，然后本局分数清零。
    /// 建议在玩家死亡逻辑 / GameOver界面出现的地方调用一次。
    /// </summary>
    public void EndRun()
    {
        TotalCoins += runScore;
        runScore = 0;
    }

    /// <summary>商城里花钱升级时调用，返回是否扣款成功（余额不够会失败）</summary>
    public bool TrySpendCoins(int amount)
    {
        if (TotalCoins < amount) return false;
        TotalCoins -= amount;
        return true;
    }

    /// <summary>重新开始新的一局时调用，清空本局分数（不影响已存档的总金币）</summary>
    public void ResetRunScore()
    {
        runScore = 0;
    }
}