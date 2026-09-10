using UnityEngine;
public static class PlayerUpgradeData
{
    private const string SPEED_LEVEL_KEY = "SpeedLevel";
    private const string WEIGHT_LEVEL_KEY = "WeightLevel";
    private const string HUNGER_LEVEL_KEY = "HungerLevel";

    public const int MaxSpeedLevel = 3;
    public const int MaxWeightLevel = 3;
    public const int MaxHungerLevel = 3;

    private const int SpeedBaseCost = 50;
    private const float SpeedCostMultiplier = 1.3f;

    private const int WeightBaseCost = 80;
    private const float WeightCostMultiplier = 1.35f;

    private const int HungerBaseCost = 60;
    private const float HungerCostMultiplier = 1.3f;

    private const float SpeedBonusPerLevel = 0.5f;  
    private const float WeightBonusPerLevel = 1.0f; 
    private const float HungerBonusPerLevel = 20f;   


    public static int SpeedLevel
    {
        get => PlayerPrefs.GetInt(SPEED_LEVEL_KEY, 0);
        private set => PlayerPrefs.SetInt(SPEED_LEVEL_KEY, value);
    }

    public static int GetNextSpeedCost()
    {
        if (SpeedLevel >= MaxSpeedLevel) return -1;
        return Mathf.RoundToInt(SpeedBaseCost * Mathf.Pow(SpeedCostMultiplier, SpeedLevel));
    }

    public static float GetSpeedBonus()
    {
        return SpeedLevel * SpeedBonusPerLevel;
    }

    public static bool TryUpgradeSpeed()
    {
        int cost = GetNextSpeedCost();
        if (cost < 0) return false; // 已满级

        if (!ScoreManager.Instance.TrySpendCoins(cost)) return false;

        SpeedLevel += 1;
        PlayerPrefs.Save();
        return true;
    }

    //Weight

    public static int WeightLevel
    {
        get => PlayerPrefs.GetInt(WEIGHT_LEVEL_KEY, 0);
        private set => PlayerPrefs.SetInt(WEIGHT_LEVEL_KEY, value);
    }

    public static int GetNextWeightCost()
    {
        if (WeightLevel >= MaxWeightLevel) return -1;
        return Mathf.RoundToInt(WeightBaseCost * Mathf.Pow(WeightCostMultiplier, WeightLevel));
    }

    public static float GetWeightBonus()
    {
        return WeightLevel * WeightBonusPerLevel;
    }

    public static bool TryUpgradeWeight()
    {
        int cost = GetNextWeightCost();
        if (cost < 0) return false;

        if (!ScoreManager.Instance.TrySpendCoins(cost)) return false;

        WeightLevel += 1;
        PlayerPrefs.Save();
        return true;
    }

    //Hunger

    public static int HungerLevel
    {
        get => PlayerPrefs.GetInt(HUNGER_LEVEL_KEY, 0);
        private set => PlayerPrefs.SetInt(HUNGER_LEVEL_KEY, value);
    }

    public static int GetNextHungerCost()
    {
        if (HungerLevel >= MaxHungerLevel) return -1;
        return Mathf.RoundToInt(HungerBaseCost * Mathf.Pow(HungerCostMultiplier, HungerLevel));
    }


    public static float GetHungerBonus()
    {
        return HungerLevel * HungerBonusPerLevel;
    }

    public static bool TryUpgradeHunger()
    {
        int cost = GetNextHungerCost();
        if (cost < 0) return false;

        if (!ScoreManager.Instance.TrySpendCoins(cost)) return false;

        HungerLevel += 1;
        PlayerPrefs.Save();
        return true;
    }
}