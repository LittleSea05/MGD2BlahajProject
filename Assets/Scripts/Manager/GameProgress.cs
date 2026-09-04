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
        return levelIndex ;
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex <= 0) return true; 
        return GetTreasureAmount(levelIndex - 1) >= RequiredAmount(levelIndex);
    }

}

