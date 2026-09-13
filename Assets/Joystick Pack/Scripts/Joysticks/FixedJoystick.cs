using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    [Header("Follow Settings")]
    [SerializeField] private float followStrength = 1f;

    private RectTransform joystickRect;
    private RectTransform movementArea;

    private Canvas joystickCanvas;
    private Camera joystickCamera;

    private Vector2 lastPointerPosition;

    protected override void Start()
    {
        base.Start();

        joystickRect = GetComponent<RectTransform>();
        movementArea = transform.parent as RectTransform;
        joystickCanvas = GetComponentInParent<Canvas>();

        if (movementArea == null)
        {
            Debug.LogError("Fixed Joystick needs a RectTransform parent as movement area.");
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        lastPointerPosition = eventData.position;

        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 pointerDelta = eventData.position - lastPointerPosition;

        base.OnDrag(eventData);

        if (Direction.magnitude >= 0.95f)
        {
            MoveBackground(pointerDelta);
        }

        lastPointerPosition = eventData.position;
    }

    private void MoveBackground(Vector2 screenDelta)
    {
        if (movementArea == null || joystickCanvas == null)
            return;

        Vector2 canvasDelta =
            screenDelta / joystickCanvas.scaleFactor * followStrength;

        Vector2 newPosition =
            joystickRect.anchoredPosition + canvasDelta;

        Rect areaRect = movementArea.rect;
        Rect joystickArea = joystickRect.rect;

        float halfWidth = joystickArea.width * 0.5f;
        float halfHeight = joystickArea.height * 0.5f;

        float minX = areaRect.xMin + halfWidth;
        float maxX = areaRect.xMax - halfWidth;

        float minY = areaRect.yMin + halfHeight;
        float maxY = areaRect.yMax - halfHeight;

        newPosition.x = Mathf.Clamp(
            newPosition.x,
            minX,
            maxX
        );

        newPosition.y = Mathf.Clamp(
            newPosition.y,
            minY,
            maxY
        );

        joystickRect.anchoredPosition = newPosition;
    }
}