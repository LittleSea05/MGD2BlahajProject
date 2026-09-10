using UnityEngine;

/// <summary>
/// 简化版：平时朝一个随机方向慢慢游，隔几秒换个方向；
/// 玩家进入侦测范围就直接冲过去，贴近了就咬一口（带冷却）。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AggressiveFishAI : MonoBehaviour
{
    public float detectRange = 10f;
    public float chaseSpeed = 6f;
    public float biteRange = 1f;
    public float biteDamage = 10f;
    public float biteInterval = 1f;

    public float wanderSpeed = 4f;
    public float changeDirectionInterval = 2f;

    public float tiltSpeed = 5f;
    public float maxTiltAngle = 15f;

    private Rigidbody rb;
    private Transform player;
    private Vector3 wanderDirection;
    private float changeDirTimer;
    private float biteCooldownTimer;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        PickNewWanderDirection();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (biteCooldownTimer > 0f) biteCooldownTimer -= Time.fixedDeltaTime;

        float distanceToPlayer = Mathf.Infinity;
        if (player != null)
        {
            Vector3 diff = player.position - transform.position;
            diff.z = 0f;
            distanceToPlayer = diff.magnitude;
        }

        Vector3 moveDirection = (player != null && distanceToPlayer <= detectRange)
            ? ChasePlayer(distanceToPlayer)
            : Wander();

        FaceDirection(moveDirection);
    }

    Vector3 ChasePlayer(float distanceToPlayer)
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.z = 0f;
        Vector3 direction = toPlayer.normalized;

        if (distanceToPlayer <= biteRange)
        {
            rb.linearVelocity = Vector3.zero;
            if (biteCooldownTimer <= 0f)
            {
                if (HealthSlider.Instance != null)
                {
                    HealthSlider.Instance.AddHealth(-biteDamage);
                }
                biteCooldownTimer = biteInterval;
            }
        }
        else
        {
            rb.linearVelocity = direction * chaseSpeed;
        }

        return direction;
    }

    Vector3 Wander()
    {
        changeDirTimer -= Time.fixedDeltaTime;
        if (changeDirTimer <= 0f)
        {
            PickNewWanderDirection();
        }

        rb.linearVelocity = wanderDirection * wanderSpeed;
        return wanderDirection;
    }

    void PickNewWanderDirection()
    {

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        wanderDirection = new Vector3(randomDir.x, randomDir.y, 0f);
        changeDirTimer = changeDirectionInterval;
    }

    void FaceDirection(Vector3 moveDirection)
    {
        if (Mathf.Abs(moveDirection.x) > 0.1f)
        {
            facingRight = moveDirection.x < 0;
            Vector3 scale = transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        float tiltAngle = -moveDirection.y * maxTiltAngle;
        if (!facingRight) tiltAngle = -tiltAngle;

        Quaternion targetTilt = Quaternion.Euler(0, 0, tiltAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTilt, tiltSpeed * Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = new Color(1f, 0.3f, 0f);
        Gizmos.DrawWireSphere(transform.position, biteRange);
    }
}