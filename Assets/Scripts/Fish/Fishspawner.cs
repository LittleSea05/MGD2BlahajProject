using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定时在场景范围内生成NPC鱼。挂在一个空物体上即可。
/// 可以配置多种鱼的预制体（比如小鱼、中鱼），按权重随机生成，
/// 制造出"有大有小"的效果，方便做体型比较的玩法。
/// </summary>
public class FishSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FishEntry
    {
        public GameObject fishPrefab;
        public float weight = 1f;
    }

    [Header("Types of Fish")]
    public List<FishEntry> fishTypes = new List<FishEntry>();

    [Header("Spawn Point")]
    public Transform spawnAreaCenter;
    public Vector2 spawnAreaSize = new Vector2(15f, 8f);

    [Header("Spawn Rate")]
    public int maxFishCount = 20;
    public float spawnInterval = 1.5f;

    private readonly List<GameObject> spawnedFish = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 清理已经被吃掉/销毁的鱼
            spawnedFish.RemoveAll(f => f == null);

            if (spawnedFish.Count < maxFishCount && fishTypes.Count > 0)
            {
                SpawnOneFish();
            }
        }
    }

    void SpawnOneFish()
    {
        GameObject prefab = PickWeightedRandom();
        if (prefab == null) return;

        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        float x = Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f);
        float y = Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f);
        Vector3 spawnPos = center + new Vector3(x, y, 0f);

        GameObject fish = Instantiate(prefab, spawnPos, Quaternion.identity);

        // 把生成范围同步给FishAI，让鱼在同一片区域里游走
        FishAI ai = fish.GetComponent<FishAI>();
        if (ai != null)
        {
            ai.areaCenter = spawnAreaCenter != null ? spawnAreaCenter : transform;
            ai.areaSize = spawnAreaSize;
        }

        spawnedFish.Add(fish);
    }

    GameObject PickWeightedRandom()
    {
        float totalWeight = 0f;
        foreach (var entry in fishTypes) totalWeight += entry.weight;
        if (totalWeight <= 0f) return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var entry in fishTypes)
        {
            cumulative += entry.weight;
            if (roll <= cumulative) return entry.fishPrefab;
        }
        return fishTypes[fishTypes.Count - 1].fishPrefab;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
    }
}