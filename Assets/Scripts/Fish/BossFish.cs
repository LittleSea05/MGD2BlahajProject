using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossFish : MonoBehaviour
{
    [Header("Stunning")]
    public int requiredHits = 5;
    public float hitCooldown = 0.5f;

    [Header("Weak")]
    [Range(0f, 1f)]
    public float weakSpeedMultiplier = 0.3f;

    [Header("Attack")]
    public float detectRange = 8f;
    public float chaseSpeed = 3.5f;
    public float biteRange = 1.2f;
    public float biteDamage = 20f;
    public float biteInterval = 1.5f;

    [Header("Wander")]
    public Transform areaCenter;
    public Vector2 areaSize = new Vector2(15f, 8f);
    public float wanderSpeed = 1.5f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 4f;
    public float arriveDistance = 0.5f;
    public float tiltSpeed = 5f;
    public float maxTiltAngle = 12f;

    [Header("Eaten")]
    public GameObject victoryPanel;
    public int scoreValue = 500;

   
    public bool IsWeak { get; private set; } = false;

    private Rigidbody rb;
    private Transform player;
    private Vector3 centerPos;
    private Vector3 wanderTarget;
    private float wanderWaitTimer;
    private float biteCooldownTimer;
    private float hitCooldownTimer;
    private int currentHits = 0;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        Renderer renderer = GetComponent<Renderer>();

        centerPos = areaCenter != null ? areaCenter.position : transform.position;
        PickNewWanderTarget();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("BossFish: 场景里找不到Tag为Player的物体，追击/撕咬行为不会生效。");
        }
    }

    void FixedUpdate()
    {
        if (hitCooldownTimer > 0f) hitCooldownTimer -= Time.fixedDeltaTime;
        if (biteCooldownTimer > 0f) biteCooldownTimer -= Time.fixedDeltaTime;

        float distanceToPlayer = player != null
            ? Vector3.Distance(transform.position, player.position)
            : Mathf.Infinity;

        Vector3 moveDirection;

        
        if (!IsWeak && player != null && distanceToPlayer <= detectRange)
        {
            moveDirection = ChasePlayer(distanceToPlayer);
        }
        else
        {
            float speed = IsWeak ? wanderSpeed * weakSpeedMultiplier : wanderSpeed;
            moveDirection = Wander(speed);
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

    Vector3 Wander(float speed)
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
        rb.linearVelocity = direction * speed;
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


    public void OnRushHitByPlayer()
    {
        if (IsWeak) return;             
        if (hitCooldownTimer > 0f) return; 

        currentHits++;
        hitCooldownTimer = hitCooldown;

     
        if (currentHits >= requiredHits)
        {
            IsWeak = true;
            
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.color = Color.red;
  
        }
    }

  
    public void GetEaten()
    {
        if (!IsWeak) return;

        if (ScoreManager.Instance != null && scoreValue > 0)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, biteRange);
    }
}