using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Fish : MonoBehaviour
{
    
    public float fishSize = 1f;

    [Header("score")]
    [Tooltip("score get")]
    public int scoreValue = 10;

    [Tooltip("weight")]
    public float growthOnEat = 0.05f;

    [Tooltip("health")]
    public float healthRestoreOnEat = 5f;

    [Header("effect")]
    public GameObject eatEffectPrefab;

    [Header("sfx")]
    public AudioClip eatSfx;

    public void GetEaten()
    {
        if (eatEffectPrefab != null)
        {
            Instantiate(eatEffectPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.Instance != null && eatSfx != null)
        {
            AudioManager.Instance.PlaySFX(eatSfx);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        if (HealthSlider.Instance != null)
        {
            HealthSlider.Instance.AddHealth(healthRestoreOnEat);
        }

        Destroy(gameObject);
    }



}