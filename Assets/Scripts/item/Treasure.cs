using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Treasure : MonoBehaviour
{
    public string treasureId;

    public int levelIndex;

    void Start()
    {
        if (string.IsNullOrEmpty(treasureId))
        {
            Debug.LogWarning($"Treasure on {gameObject.name} 没有设置 treasureId！");
            return;
        }

        if (GameProgress.IsTreasureCollected(treasureId))
        {
            Destroy(gameObject);
        }
    }

    public void Collect()
    {
        if (string.IsNullOrEmpty(treasureId)) return;
        if (GameProgress.IsTreasureCollected(treasureId)) return; 

        GameProgress.MarkTreasureCollected(treasureId);

        int newCount = GameProgress.GetTreasureAmount(levelIndex) + 1;
        GameProgress.SaveTreasureAmount(levelIndex, newCount);

        Destroy(gameObject);
    }
}