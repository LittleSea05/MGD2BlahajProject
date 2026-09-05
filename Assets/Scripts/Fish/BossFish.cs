using UnityEngine;

/// <summary>
/// Boss鱼。跟普通NPC鱼(Fish.cs)完全分开一套逻辑，因为规则不一样：
/// - 平时玩家不能吃它，撞上去也没用（除非是rush状态）
/// - 玩家用rush撞它，撞满 requiredHits 次后，它进入"虚弱"状态
/// - 只有虚弱状态才能被玩家吃掉，吃掉后触发胜利面板（"大海恢复平静"）
/// - 没进入虚弱状态之前，它会主动追击、撕咬玩家（类似AggressiveFishAI，但独立实现，
///   方便boss以后加更多专属状态，不用跟普通攻击鱼共用一个脚本）
///
/// 场景设置要点：
/// - 给这条鱼建一个新Tag，比如"Boss"，不要用"NPCFish"（不然会被当成普通鱼直接吃掉的判定抢走）
/// - PlayerControl的OnTriggerEnter里要加一段专门处理"Boss"标签的逻辑（见下方说明）
/// </summary>
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

    /// <summary>是否已经虚弱，虚弱了才能被吃掉。PlayerControl靠这个字段判断。</summary>
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

        // 虚弱之后就不再主动攻击玩家了，只是慢悠悠地游走，方便玩家上去吃掉它
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

    /// <summary>
    /// 玩家在rush状态下撞到boss时调用（由PlayerControl触发）。
    /// 撞满requiredHits次后boss进入虚弱状态。
    /// </summary>
    public void OnRushHitByPlayer()
    {
        if (IsWeak) return;              // 已经虚弱了，不用再撞
        if (hitCooldownTimer > 0f) return; // 冷却中，避免一次贴脸判定好几下

        currentHits++;
        hitCooldownTimer = hitCooldown;

        // 受击反馈可以加在这里，比如受击特效、屏幕震动、音效：
        // Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (currentHits >= requiredHits)
        {
            IsWeak = true;
            
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.color = Color.red;
            // 进入虚弱状态的额外表现（比如换个"晕眩"贴图）也可以加在这里
        }
    }

    /// <summary>boss处于虚弱状态时，玩家把它吃掉，调用这个方法（由PlayerControl触发）。</summary>
    public void GetEaten()
    {
        if (!IsWeak) return; // 保险检查：没虚弱不能吃

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