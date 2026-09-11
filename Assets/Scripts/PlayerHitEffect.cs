using UnityEngine;
using System.Collections;

public class PlayerHitEffect : MonoBehaviour
{
    public static PlayerHitEffect Instance { get; private set; }

    public ParticleSystem targetParticleSystem;

    [Header("value")]
    public Color hitColor = Color.red;
    public float emissionMultiplier = 3f;
    public float effectDuration = 0.4f;

    private Color originalColor;
    private float originalRateMultiplier;
    private Coroutine currentEffect;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    void Awake()
    {
        Instance = this;

        if (targetParticleSystem == null)
        {
            targetParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        if (targetParticleSystem != null)
        {
            mainModule = targetParticleSystem.main;
            emissionModule = targetParticleSystem.emission;
            originalColor = mainModule.startColor.color;
            originalRateMultiplier = emissionModule.rateOverTimeMultiplier;
        }

    }

    public void TriggerHitEffect()
    {
        if (targetParticleSystem == null) return;

        if (currentEffect != null)
        {
            StopCoroutine(currentEffect);
        }
        currentEffect = StartCoroutine(HitEffectRoutine());
    }

    IEnumerator HitEffectRoutine()
    {
        mainModule.startColor = hitColor;
        emissionModule.rateOverTimeMultiplier = originalRateMultiplier * emissionMultiplier;

        yield return new WaitForSeconds(effectDuration);

        mainModule.startColor = originalColor;
        emissionModule.rateOverTimeMultiplier = originalRateMultiplier;

        currentEffect = null;
    }
}