using UnityEngine;
 
public class CameraFollow : MonoBehaviour
{
    public Transform target;           // 要跟隨的玩家
    public Vector3 offset = new Vector3(0f, 2f, -10f);
    public float smoothSpeed = 5f;
 
    void LateUpdate()
    {
        if (target == null) return;
 
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}