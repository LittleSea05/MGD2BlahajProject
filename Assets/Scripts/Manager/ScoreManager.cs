using UnityEngine;
using System;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const string TOTAL_COINS_KEY = "TotalCoins";

    [Tooltip("current score")]
    public int runScore = 0;

    [Tooltip("score to coin with 1.5x")]
    public float scoreToCoinMultiplier = 1.5f;

   
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

   
    public void AddScore(int amount)
    {
        runScore += amount;
        OnScoreChanged?.Invoke(runScore);
    }


    public int TotalCoins
    {
        get => PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        private set
        {
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public int getMultiplyCoin()
    {
        return Mathf.RoundToInt(runScore * scoreToCoinMultiplier);
    }


    public void EndRun()
    {
        TotalCoins += getMultiplyCoin();
    }

   
    public bool TrySpendCoins(int amount)
    {
        if (TotalCoins < amount) return false;
        TotalCoins -= amount;
        return true;
    }

    
    public void ResetRunScore()
    {
        runScore = 0;
    }

    private string GetHighScoreKey(int levelIndex)
    {
        return $"HighScore_Level{levelIndex}";
    }

    public int UpdateHighScore(int levelIndex, int finalScore)
    {
        string key = GetHighScoreKey(levelIndex);
        int savedHigh = PlayerPrefs.GetInt(key, 0);

        if (finalScore > savedHigh)
        {
            savedHigh = finalScore;
            PlayerPrefs.SetInt(key, savedHigh);
            PlayerPrefs.Save();
        }

        return savedHigh;
    }

    public int GetHighScore(int levelIndex)
    {
        return PlayerPrefs.GetInt(GetHighScoreKey(levelIndex), 0);
    }
}