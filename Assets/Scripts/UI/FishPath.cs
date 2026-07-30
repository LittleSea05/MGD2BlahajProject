using UnityEngine;
using DG.Tweening;

public class FishPath : MonoBehaviour
{
    public Transform[] waypoints;
    public float duration = 10f;

    void Start()
    {
        Vector3[] path = new Vector3[waypoints.Length];

        for (int i = 0; i < waypoints.Length; i++)
        {
            path[i] = waypoints[i].position;
        }

        transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetLookAt(0.01f);
    }
}