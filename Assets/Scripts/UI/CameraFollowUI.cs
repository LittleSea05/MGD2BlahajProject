using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CameraFollowUI : MonoBehaviour
{
    public Transform[] cameraPoints;
    public float moveTime = 1.5f;

    [Header("Swipe Look")]
    public float rotateSpeed = 0.2f;
    public float mouseRotateSpeed = 3f;
    public float maxYawOffset = 360f;
    public float maxPitchOffset = 360f;

    private Quaternion baseRotation;
    private float yawOffset = 0f;
    private float pitchOffset = 0f;

    private int activeTouchId = -1;
    private Vector2 lastTouchPos;

    private bool isDraggingMouse = false;

    void Start()
    {
        if (cameraPoints != null && cameraPoints.Length > 0)
        {
            transform.position = cameraPoints[0].position;
            transform.rotation = cameraPoints[0].rotation;

            baseRotation = cameraPoints[0].rotation;
        }
        else
        {
            baseRotation = transform.rotation;
        }
    }

    public void MoveToLevel(int levelIndex)
    {
        if (cameraPoints == null ||
            levelIndex < 0 ||
            levelIndex >= cameraPoints.Length)
            return;

        Transform target = cameraPoints[levelIndex];

        yawOffset = 0f;
        pitchOffset = 0f;

        transform.DOKill();

        transform.DOMove(target.position, moveTime)
            .SetEase(Ease.InOutSine);

        transform.DORotateQuaternion(target.rotation, moveTime)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                baseRotation = transform.rotation;
            })
            .OnComplete(() =>
            {
                baseRotation = target.rotation;
            });
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleSwipeInput();
#endif

        Quaternion offsetRot =
            Quaternion.Euler(-pitchOffset, yawOffset, 0);

        transform.rotation = baseRotation * offsetRot;
    }

    void HandleMouseInput()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            isDraggingMouse = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDraggingMouse = true;
            lastTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraggingMouse = false;
        }
        else if (isDraggingMouse && Input.GetMouseButton(0))
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 delta = currentPos - lastTouchPos;

            lastTouchPos = currentPos;

            yawOffset = Mathf.Clamp(
                yawOffset + delta.x * mouseRotateSpeed * 0.1f,
                -maxYawOffset,
                maxYawOffset
            );

            pitchOffset = Mathf.Clamp(
                pitchOffset + delta.y * mouseRotateSpeed * 0.1f,
                -maxPitchOffset,
                maxPitchOffset
            );
        }
    }

    void HandleSwipeInput()
    {
        if (Input.touchCount == 0)
        {
            activeTouchId = -1;
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                continue;
            }

            if (touch.phase == TouchPhase.Began)
            {
                if (activeTouchId == -1)
                {
                    activeTouchId = touch.fingerId;
                    lastTouchPos = touch.position;
                }
            }
            else if (touch.fingerId == activeTouchId)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta =
                        touch.position - lastTouchPos;

                    lastTouchPos = touch.position;

                    yawOffset = Mathf.Clamp(
                        yawOffset + delta.x * rotateSpeed,
                        -maxYawOffset,
                        maxYawOffset
                    );

                    pitchOffset = Mathf.Clamp(
                        pitchOffset + delta.y * rotateSpeed,
                        -maxPitchOffset,
                        maxPitchOffset
                    );
                }
                else if (
                    touch.phase == TouchPhase.Ended ||
                    touch.phase == TouchPhase.Canceled)
                {
                    activeTouchId = -1;
                }
            }
        }
    }
}