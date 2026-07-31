using UnityEngine;
using UnityEngine.UI;

public class StaminaSlider : MonoBehaviour
{
    public Slider staminaSlider;
    public float maxStam = 100f;
    public float drainRate = 50f;
    public float fillRate = 15f;
    public float fillDelay = 2f;

    private float currentStam;
    private float fillTimer;

    public static bool HasStamina => instance != null && instance.currentStam > 0f;
    private static StaminaSlider instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentStam = maxStam;
        UpdateSlider();
    }

    void Update()
    {
        bool isRushing = RushButton.IsRushing && currentStam > 0f;

        if (isRushing)
        {
            currentStam -= drainRate * Time.deltaTime;
            currentStam = Mathf.Max(currentStam, 0f);
            fillTimer = 0f;
        }
        else
        {
            fillTimer += Time.deltaTime;

            if (fillTimer >= fillDelay)
            {
                currentStam += fillRate * Time.deltaTime;
                currentStam = Mathf.Min(currentStam, maxStam);
            }
        }

        UpdateSlider();
    }

    void UpdateSlider()
    {
        staminaSlider.value = currentStam / maxStam;
    }
}