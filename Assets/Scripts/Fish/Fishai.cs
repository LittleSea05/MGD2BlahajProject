using UnityEngine;

/// <summary>
/// 让NPC鱼在指定范围内随机游动。
/// 逻辑：每隔一段时间，在游走范围内随机选一个目标点，朝那个点游过去，
/// 到达后（或超时后）再选下一个目标点。移动时根据水平方向翻转贴图朝向，
/// 跟PlayerControl里的翻转逻辑保持一致。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FishAI : MonoBehaviour
{

    public Transform areaCenter;
    public Vector2 areaSize = new Vector2(10f, 6f);

    [Header("Moving")]
    public float moveSpeed = 2f;
    public float tiltSpeed = 5f;
    public float maxTiltAngle = 15f;
    public float minWaitTime = 1.5f;
    public float maxWaitTime = 4f;

    public float arriveDistance = 0.3f;

    private Rigidbody rb;
    private Vector3 centerPos;
    private Vector3 targetPoint;
    private bool facingRight = true;
    private float waitTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        centerPos = areaCenter != null ? areaCenter.position : transform.position;
        PickNewTarget();
    }

    void FixedUpdate()
    {
        Vector3 toTarget = targetPoint - transform.position;
        toTarget.z = 0f;

        // 到达目标点或者等待时间到了，就换一个新目标点
        waitTimer -= Time.fixedDeltaTime;
        if (toTarget.magnitude <= arriveDistance || waitTimer <= 0f)
        {
            PickNewTarget();
            toTarget = targetPoint - transform.position;
            toTarget.z = 0f;
        }

        Vector3 moveDirection = toTarget.normalized;
        rb.linearVelocity = moveDirection * moveSpeed;

        // 左右翻转，跟玩家脚本保持一致的写法
        if (Mathf.Abs(moveDirection.x) > 0.1f)
        {
            facingRight = moveDirection.x < 0;
            Vector3 scale = transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // 上下移动时轻微倾斜，效果更自然
        float tiltAngle = -moveDirection.y * maxTiltAngle;
        if (!facingRight) tiltAngle = -tiltAngle;

        Quaternion targetTilt = Quaternion.Euler(0, 0, tiltAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTilt, tiltSpeed * Time.fixedDeltaTime);
    }

    void PickNewTarget()
    {
        float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float y = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
        targetPoint = centerPos + new Vector3(x, y, 0f);
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    // 方便在Scene视图里看到游走范围，调试用
    void OnDrawGizmosSelected()
    {
        Vector3 center = areaCenter != null ? areaCenter.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, areaSize.y, 0.1f));
    }
}