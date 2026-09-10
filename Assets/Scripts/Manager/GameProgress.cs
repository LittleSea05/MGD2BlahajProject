using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static void SaveTreasureAmount(int levelIndex, int amount)
    {
        PlayerPrefs.SetInt($"Level{levelIndex}_TreasureAmount", amount);
        PlayerPrefs.Save();
    }

    public static int GetTreasureAmount(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level{levelIndex}_TreasureAmount", 0);
    }

    public static int RequiredAmount(int levelIndex)
    {
        return levelIndex + 1; // Tutorial(0)需要1个, Level1(1)需要2个...
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex <= 0) return true;
        return GetTreasureAmount(levelIndex - 1) >= RequiredAmount(levelIndex - 1);
    }


    public static bool IsTreasureCollected(string treasureId)
    {
        return PlayerPrefs.GetInt($"Treasure_{treasureId}", 0) == 1;
    }


    public static void MarkTreasureCollected(string treasureId)
    {
        PlayerPrefs.SetInt($"Treasure_{treasureId}", 1);
        PlayerPrefs.Save();
    }
}