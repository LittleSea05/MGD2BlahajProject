// using UnityEngine;

// public static class GameBootstrapper
// {
//     private static bool initialized = false;

//     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     static void Init()
//     {
//         if (initialized) return;
//         initialized = true;

//         EnsureExists("ScoreManager");
//         EnsureExists("BrightnessCanvas");

//     }

//     static void EnsureExists(string prefabName)
//     {
//         GameObject prefab = Resources.Load<GameObject>(prefabName);
//         if (prefab == null)
//         {
//             Debug.LogWarning($"GameBootstrapper: 找不到 Resources/{prefabName}.prefab");
//             return;
//         }

//         GameObject instance = Object.Instantiate(prefab);
//         instance.name = prefabName;
//     }
// }