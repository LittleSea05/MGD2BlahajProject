using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [Header("recent score")]
    public TMP_Text liveScoreText;

    [Header("die panel: final score(optional)")]
    public TMP_Text finalScoreText;

    [Header("die panel: coins earned this run")]
    public TMP_Text coinsText;

    [Header("Highscore")]
    public TMP_Text highScoreText;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            if (liveScoreText != null)
            {
                UpdateLiveScore(ScoreManager.Instance.runScore);
                ScoreManager.Instance.OnScoreChanged += UpdateLiveScore;
            }
        }
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


    public void RefreshCoins()
    {
        if (coinsText != null && ScoreManager.Instance != null)
        {
            coinsText.text = $"Coins: {ScoreManager.Instance.TotalCoins}";
        }
    }


    public void ShowGameOverResults(int finalScore, int levelIndex)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {finalScore}";
        }

        if (coinsText != null && ScoreManager.Instance != null)
        {
            int finalCoins = ScoreManager.Instance.getMultiplyCoin();
            coinsText.text = $"Coins: {finalCoins}";
        }

        if (highScoreText != null && ScoreManager.Instance != null)
        {
            int best = ScoreManager.Instance.UpdateHighScore(levelIndex, finalScore);
            highScoreText.text = $"Best: {best}";
        }
    }
}