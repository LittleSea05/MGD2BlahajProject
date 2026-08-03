using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSlider : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth = 100f;
    public float drainRate = 5f; 
    public GameObject panel;

    private float currentHealth;
    private float Timer;
    private bool stopGame;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateSlider();
    }

    void Update()
    {
        currentHealth -= drainRate * Time.deltaTime;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateSlider();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateSlider()
    {
        healthSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        panel.SetActive(true);
        Time.timeScale=0f;

    }

    public void backMenu()
    {
        SceneManager.LoadScene("SelectLevel");
    }
}
