using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    private Rigidbody rb;
    public float rushSpeed = 15f;
    public Joystick playerJoystick;

    public float moveSpeed = 5f;
    public float tiltSpeed = 8f;
    public float maxTiltAngle = 20f;
    public float collectTreasure;
    public int currentLevelIndex;

    private Vector3 moveDirection;
    private bool facingRight = true;

    public float currentSize = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        collectTreasure = 0;
    }

    void FixedUpdate()
    {
        float horizontal = playerJoystick.Horizontal;
        float vertical = playerJoystick.Vertical;

        //Debug.Log($"horizontal={horizontal}, facingRight={facingRight}, scale.x={transform.localScale.x}");

        moveDirection = new Vector3(horizontal, vertical, 0).normalized;

        float currentSpeed = moveSpeed;
        if (moveDirection != Vector3.zero && RushButton.IsRushing && StaminaSlider.HasStamina)
        {
            currentSpeed = rushSpeed;
        }

        rb.linearVelocity = moveDirection * currentSpeed;

        // 左右翻转,直接在自己身上做
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            facingRight = horizontal < 0;
            Vector3 scale = transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // 上下移动时的轻微倾斜
        float tiltAngle = -vertical * maxTiltAngle;
        if (!facingRight) tiltAngle = -tiltAngle;

        Quaternion targetTilt = Quaternion.Euler(0, 0, tiltAngle);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetTilt,
            tiltSpeed * Time.fixedDeltaTime
        );

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Treasure"))
    {
        Destroy(other.gameObject);
        collectTreasure += 1;
        CollectManager.Instance.showPickUp();
        GameProgress.SaveTreasureAmount(currentLevelIndex, (int)collectTreasure);
    }
    else if (other.gameObject.CompareTag("NPCFish"))
    {
        Fish fish = other.GetComponent<Fish>();
        if (fish == null) return;

        if (currentSize >= fish.fishSize)
        {
            // 玩家更大，吃掉它
            currentSize += fish.growthOnEat;
            fish.GetEaten();
        }
        else
        {
            // 玩家更小，被反咬一口，具体逻辑你们自己定
            // 比如：PlayerHealth.Instance.TakeDamage(...)
        }
    }else if (other.gameObject.CompareTag("Boss"))
    {
    BossFish boss = other.GetComponent<BossFish>();
    if (boss == null) return;

    if (boss.IsWeak)
    {
        // 已经虚弱了，吃掉它
        boss.GetEaten();
    }
    else if (RushButton.IsRushing)
    {
        // 冲刺状态下撞到它，算一次有效撞击
        boss.OnRushHitByPlayer();
    }
    // 没有在rush、boss也没虚弱：什么都不做。
    // boss会不会咬玩家，是BossFish自己每帧检测距离来判断的，不依赖这个触发器，
    // 所以这里不用担心"漏处理"玩家被咬的情况
    }
    }
}

