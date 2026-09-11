using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerDamageFlash : MonoBehaviour
{
    public static PlayerDamageFlash Instance { get; private set; }

    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;

    private MeshRenderer meshRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        Instance = this;
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    // 被咬时调用这个
    public void TriggerDamageFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        meshRenderer.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        meshRenderer.material.color = originalColor;
        flashCoroutine = null;
    }
}