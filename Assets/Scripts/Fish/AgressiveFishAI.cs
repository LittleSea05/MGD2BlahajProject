using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class AggressiveFishAI : MonoBehaviour
{
    [Header("Attack")]
    public float detectRange = 10f;
    public float chaseSpeed = 5f;
    public float biteRange = 1f;
    public float biteDamage = 10f;
    public float biteInterval = 1f;

    [Header("Wander")]
    public Transform areaCenter;
    public Vector2 areaSize = new Vector2(10f, 6f);
    public float wanderSpeed = 3f;
    public float minWaitTime = 1.5f;
    public float maxWaitTime = 4f;
    public float arriveDistance = 0.3f;
    public float tiltSpeed = 5f;
    public float maxTiltAngle = 15f;

    private Rigidbody rb;
    private Transform player;
    private Vector3 centerPos;
    private Vector3 wanderTarget;
    private float wanderWaitTimer;
    private float biteCooldownTimer;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        centerPos = areaCenter != null ? areaCenter.position : transform.position;
        PickNewWanderTarget();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"{gameObject.name} FindPlayer {playerObj.name}");
        }
        else
        {
            Debug.LogWarning("AggressiveFishAI: No GameObject with tag 'Player' found. Please ensure the player has the correct tag.");
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (biteCooldownTimer > 0f)
        {
            biteCooldownTimer -= Time.fixedDeltaTime;
        }

        float distanceToPlayer = Mathf.Infinity;
        if (player != null)
        {
            Vector3 diff = player.position - transform.position;
            diff.z = 0f;
            distanceToPlayer = diff.magnitude;   
        }

        Vector3 moveDirection;

        if (player != null && distanceToPlayer <= detectRange)
        {
            moveDirection = ChasePlayer(distanceToPlayer);
        }
        else
        {
            moveDirection = Wander();
        }

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
                BitePlayer();
                biteCooldownTimer = biteInterval;
            }
        }
        else
        {
            rb.linearVelocity = direction * chaseSpeed;
        }

        return direction;
    }

    void BitePlayer()
    {
        if (HealthSlider.Instance != null)
        {
            HealthSlider.Instance.AddHealth(-biteDamage);
        }

    }

    Vector3 Wander()
    {
        Vector3 toTarget = wanderTarget - transform.position;
        toTarget.z = 0f;

        wanderWaitTimer -= Time.fixedDeltaTime;
        if (toTarget.magnitude <= arriveDistance || wanderWaitTimer <= 0f)
        {
            PickNewWanderTarget();
            toTarget = wanderTarget - transform.position;
            toTarget.z = 0f;
        }

        Vector3 direction = toTarget.normalized;
        rb.linearVelocity = direction * wanderSpeed;
        return direction;
    }

    void PickNewWanderTarget()
    {
        float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float y = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
        wanderTarget = centerPos + new Vector3(x, y, 0f);
        wanderWaitTimer = Random.Range(minWaitTime, maxWaitTime);
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