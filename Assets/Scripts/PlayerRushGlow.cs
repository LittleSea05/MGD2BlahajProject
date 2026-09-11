using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerRushGlow : MonoBehaviour
{
    [Header("glow color")]
    [ColorUsage(true, true)]
    public Color glowColor = new Color(0.3f, 0.8f, 1f, 1f) * 3f;

    public float transitionSpeed = 8f;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    private Color currentEmission = Color.black;

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        bool isRush=RushButton.IsRushing && StaminaSlider.HasStamina;
        Color targetEmission = isRush ? glowColor : Color.black;

        currentEmission = Color.Lerp(
            currentEmission,
            targetEmission,
            transitionSpeed * Time.deltaTime
        );

        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(EmissionColorID, currentEmission);
        meshRenderer.SetPropertyBlock(propBlock);
    }
}