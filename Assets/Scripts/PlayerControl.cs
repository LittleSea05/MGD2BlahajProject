using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
     private Rigidbody rb;

    public float moveSpeed = 5f;
    public float rotateSpeed = 8f;

    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 锁住 Z，不让鱼飞出平面
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, vertical, 0).normalized;

        rb.linearVelocity = moveDirection * moveSpeed;

        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 180f);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.fixedDeltaTime
            );
        }
    }
}