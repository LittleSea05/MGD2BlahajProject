using UnityEngine;

/// <summary>
/// 纯数据/计算类，负责"速度"和"体型(weight)"两条升级线的：
/// - 当前等级（存在PlayerPrefs里，跨局保留）
/// - 升到下一级要花多少钱
/// - 当前等级对应的实际加成数值
///
/// ShopManager（商城UI）和 PlayerControl（实际应用加成）都从这里读取，
/// 好处是升级公式只写在一个地方，以后想改数值平衡只改这里。
/// </summary>
public static class PlayerUpgradeData
{
    private const string SPEED_LEVEL_KEY = "SpeedLevel";
    private const string WEIGHT_LEVEL_KEY = "WeightLevel";
    private const string HUNGER_LEVEL_KEY = "HungerLevel";

    // 等级上限：每种属性只有3级
    public const int MaxSpeedLevel = 3;
    public const int MaxWeightLevel = 3;
    public const int MaxHungerLevel = 3;

    // 花费公式参数：cost = BaseCost * Multiplier^当前等级
    private const int SpeedBaseCost = 50;
    private const float SpeedCostMultiplier = 1.3f;

    private const int WeightBaseCost = 80;
    private const float WeightCostMultiplier = 1.35f;

    private const int HungerBaseCost = 60;
    private const float HungerCostMultiplier = 1.3f;

    // 每级加成参数
    private const float SpeedBonusPerLevel = 0.5f;   // 每级 moveSpeed +0.5
    private const float WeightBonusPerLevel = 0.15f; // 每级 currentSize +0.15
    private const float HungerBonusPerLevel = 20f;   // 每级 maxHealth +20（数值按你们的血量设定改）

    // ---------------- 速度升级 ----------------

    public static int SpeedLevel
    {
        get => PlayerPrefs.GetInt(SPEED_LEVEL_KEY, 0);
        private set => PlayerPrefs.SetInt(SPEED_LEVEL_KEY, value);
    }

    /// <summary>升到下一级速度需要多少金币；已满级返回-1</summary>
    public static int GetNextSpeedCost()
    {
        if (SpeedLevel >= MaxSpeedLevel) return -1;
        return Mathf.RoundToInt(SpeedBaseCost * Mathf.Pow(SpeedCostMultiplier, SpeedLevel));
    }

    /// <summary>当前速度等级带来的移速加成，直接加到PlayerControl.moveSpeed上</summary>
    public static float GetSpeedBonus()
    {
        return SpeedLevel * SpeedBonusPerLevel;
    }

    /// <summary>尝试升级速度，成功返回true。内部会检查金币是否够、是否已满级</summary>
    public static bool TryUpgradeSpeed()
    {
        int cost = GetNextSpeedCost();
        if (cost < 0) return false; // 已满级

        if (!ScoreManager.Instance.TrySpendCoins(cost)) return false;

        SpeedLevel += 1;
        PlayerPrefs.Save();
        return true;
    }

    // ---------------- 体型(weight)升级 ----------------

    public static int WeightLevel
    {
        get => PlayerPrefs.GetInt(WEIGHT_LEVEL_KEY, 0);
        private set => PlayerPrefs.SetInt(WEIGHT_LEVEL_KEY, value);
    }

    /// <summary>升到下一级体型需要多少金币；已满级返回-1</summary>
    public static int GetNextWeightCost()
    {
        if (WeightLevel >= MaxWeightLevel) return -1;
        return Mathf.RoundToInt(WeightBaseCost * Mathf.Pow(WeightCostMultiplier, WeightLevel));
    }

    /// <summary>当前体型等级带来的体型加成，直接加到PlayerControl.currentSize上</summary>
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

    // ---------------- Hunger(生命值)升级 ----------------

    public static int HungerLevel
    {
        get => PlayerPrefs.GetInt(HUNGER_LEVEL_KEY, 0);
        private set => PlayerPrefs.SetInt(HUNGER_LEVEL_KEY, value);
    }

    /// <summary>升到下一级生命值需要多少金币；已满级返回-1</summary>
    public static int GetNextHungerCost()
    {
        if (HungerLevel >= MaxHungerLevel) return -1;
        return Mathf.RoundToInt(HungerBaseCost * Mathf.Pow(HungerCostMultiplier, HungerLevel));
    }

    /// <summary>当前生命值等级带来的加成，加到玩家的maxHealth上</summary>
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