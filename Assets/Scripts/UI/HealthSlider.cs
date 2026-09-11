using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSlider : MonoBehaviour
{
    public static HealthSlider Instance { get; private set; }

    public Slider healthSlider;
    public float maxHealth = 100f;
    public float drainRate = 5f;
    public GameObject panel;
    public int currentLevelIndex;

  
    public ScoreDisplay scoreDisplay;

    private float currentHealth;
    private float Timer;
    private bool stopGame;
    

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        stopGame = false;
        maxHealth += PlayerUpgradeData.GetHungerBonus();

        currentHealth = maxHealth;
        UpdateSlider();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetRunScore(); 
        }
    }

    void Update()
    {
        if (healthSlider == null)
        {
            enabled = false;
           
            return;
        }

        currentHealth -= drainRate * Time.deltaTime;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateSlider();

        if (currentHealth <= 0f && !stopGame)
        {
            Die();
        }
    }

    public void AddHealth(float amount)
    {
        if(amount<0f)
        {
            if(PlayerHitEffect.Instance != null)
            {
                PlayerHitEffect.Instance.TriggerHitEffect();
            }
            if(PlayerDamageFlash.Instance != null)
            {
                PlayerDamageFlash.Instance.TriggerDamageFlash();
            }
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateSlider();
    }

    void UpdateSlider()
    {
        if (healthSlider == null) return;
        healthSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        stopGame = true;
        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.runScore : 0;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndRun();
        }

        if (scoreDisplay != null)
        {
            scoreDisplay.ShowGameOverResults(finalScore, currentLevelIndex);
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void backMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SelectLevel");
    }
}