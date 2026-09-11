using UnityEngine;
using System.Collections;

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
    public Transform bitePoint;
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

    public AudioClip eatSfx;
    public AudioClip biteSfx;

    public Color hitFlashColor = Color.yellow;
    public float hitFlashDuration = 1f;

    public Transform player;

    public bool IsWeak { get; private set; } = false;

    private Rigidbody rb;
    private Vector3 centerPos;
    private Vector3 wanderTarget;
    private float wanderWaitTimer;
    private float biteCooldownTimer;
    private float hitCooldownTimer;
    private int currentHits = 0;
    private bool facingRight = true;
    private Renderer bossRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        bossRenderer = GetComponent<Renderer>();
        if (bossRenderer != null)
        {
            originalColor = bossRenderer.material.color;
        }

        centerPos = areaCenter != null ? areaCenter.position : transform.position;
        PickNewWanderTarget();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }

        }
    }

    void FixedUpdate()
    {
        if (hitCooldownTimer > 0f) hitCooldownTimer -= Time.fixedDeltaTime;
        if (biteCooldownTimer > 0f) biteCooldownTimer -= Time.fixedDeltaTime;

        float distanceToPlayer = Mathf.Infinity;
        if (player != null)
        {
            Vector3 diff = player.position - transform.position;
            diff.z = 0f;
            distanceToPlayer = diff.magnitude;
        }

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

        Vector3 biteOrigin = bitePoint != null ? bitePoint.position : transform.position;
        Vector3 biteDiff = player.position - biteOrigin;
        biteDiff.z = 0f;
        float distanceToBitePoint = biteDiff.magnitude;

        if (distanceToBitePoint <= biteRange)
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

        if (AudioManager.Instance != null && biteSfx != null)
        {
            AudioManager.Instance.PlaySFX(biteSfx);
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
        }

        TriggerHitFlash();
    }

    void TriggerHitFlash()
    {
        if (bossRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    IEnumerator HitFlashRoutine()
    {
        bossRenderer.material.color = hitFlashColor;

        yield return new WaitForSeconds(hitFlashDuration);
        bossRenderer.material.color = IsWeak ? Color.red : originalColor;

        flashCoroutine = null;
    }

    public void GetEaten()
    {
        if (!IsWeak) return;

        if (AudioManager.Instance != null && eatSfx != null)
        {
            AudioManager.Instance.PlaySFX(eatSfx);
        }

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

        Vector3 biteOrigin = bitePoint != null ? bitePoint.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(biteOrigin, biteRange);
    }
}