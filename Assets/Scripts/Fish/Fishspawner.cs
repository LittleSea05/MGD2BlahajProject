using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FishEntry
    {
        public GameObject fishPrefab;
        public float weight = 1f;

        [Tooltip("这种鱼同时最多存在几条，0 = 不限制")]
        public int maxAlive = 0;

        // 运行时追踪这种鱼当前场上存活的实例，不需要序列化
        [System.NonSerialized]
        public List<GameObject> aliveInstances = new List<GameObject>();
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

            CleanupDeadFish();

            if (spawnedFish.Count < maxFishCount && fishTypes.Count > 0)
            {
                SpawnOneFish();
            }
        }
    }

    void CleanupDeadFish()
    {
        spawnedFish.RemoveAll(f => f == null);

        foreach (var entry in fishTypes)
        {
            entry.aliveInstances.RemoveAll(f => f == null);
        }
    }

    void SpawnOneFish()
    {
        FishEntry entry = PickWeightedEntry();
        if (entry == null || entry.fishPrefab == null) return;

        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        float x = Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f);
        float y = Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f);
        Vector3 spawnPos = center + new Vector3(x, y, 0f);

        GameObject fish = Instantiate(entry.fishPrefab, spawnPos, Quaternion.identity);

        FishAI ai = fish.GetComponent<FishAI>();
        if (ai != null)
        {
            ai.areaCenter = spawnAreaCenter != null ? spawnAreaCenter : transform;
            ai.areaSize = spawnAreaSize;
        }

        spawnedFish.Add(fish);
        entry.aliveInstances.Add(fish);
    }

    // 挑选一个"还没到达存活上限"的鱼种，再按权重随机
    FishEntry PickWeightedEntry()
    {
        float totalWeight = 0f;
        List<FishEntry> eligible = new List<FishEntry>();

        foreach (var entry in fishTypes)
        {
            bool reachedCap = entry.maxAlive > 0 && entry.aliveInstances.Count >= entry.maxAlive;
            if (reachedCap) continue;

            eligible.Add(entry);
            totalWeight += entry.weight;
        }

        if (eligible.Count == 0 || totalWeight <= 0f) return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var entry in eligible)
        {
            cumulative += entry.weight;
            if (roll <= cumulative) return entry;
        }

        return eligible[eligible.Count - 1];
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
    }
}