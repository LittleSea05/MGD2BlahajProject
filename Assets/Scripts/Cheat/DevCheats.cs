// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class DevCheats : MonoBehaviour
// {
// #if UNITY_EDITOR || DEVELOPMENT_BUILD
//     [Header("按对应按键触发")]
//     public KeyCode unlockAllKey = KeyCode.F1;
//     public KeyCode jumpToLevel3Key = KeyCode.F3;

//     void Update()
//     {
//         if (Input.GetKeyDown(unlockAllKey))
//         {
//             UnlockAllLevels();
//         }

//         if (Input.GetKeyDown(jumpToLevel3Key))
//         {
//             SceneManager.LoadScene("Level3");
//         }
//     }

//     void UnlockAllLevels()
//     {
//         for (int i = 0; i < 5; i++)
//         {
//             int required = GameProgress.RequiredAmount(i);
//             GameProgress.SaveTreasureAmount(i, required);
//         }
//         Debug.Log("[DevCheats] 所有关卡解锁条件已满足");
//     }
// #endif
// }