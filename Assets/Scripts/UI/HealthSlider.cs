using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSlider : MonoBehaviour
{
    // 简单单例，方便Fish.cs等其他脚本直接调用HealthSlider.Instance.AddHealth(...)
    // 注意：这里不做DontDestroyOnLoad，它就是普通的场景内单例，场景重新加载会自然重建。
    public static HealthSlider Instance { get; private set; }

    public Slider healthSlider;
    public float maxHealth = 100f;
    public float drainRate = 5f;
    public GameObject panel;

    [Header("分数/金币显示交给ScoreDisplay统一处理")]
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
        // 加上商城里买的"生命值(Hunger)"升级加成
        maxHealth += PlayerUpgradeData.GetHungerBonus();

        currentHealth = maxHealth;
        UpdateSlider();
    }

    void Update()
    {
        // 保险检查：如果healthSlider引用丢失了（比如跨场景残留导致的失效引用），
        // 直接停止这个脚本，不要每一帧都抛异常拖垮整个场景。
        if (healthSlider == null)
        {
            enabled = false;
            Debug.LogWarning("HealthSlider: healthSlider引用丢失，已禁用该脚本。请检查是否有物体被误设成DontDestroyOnLoad。");
            return;
        }

        currentHealth -= drainRate * Time.deltaTime;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateSlider();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>吃鱼、吃道具等回血时调用。会自动clamp在0~maxHealth之间。</summary>
    public void AddHealth(float amount)
    {
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
        // 先把这局的分数记下来，因为EndRun()执行完之后runScore会被清零
        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.runScore : 0;

        // 游戏结束，把这局吃鱼攒的分数结算成金币存档
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EndRun();
        }

        // 交给ScoreDisplay统一显示本局分数 + 结算后的总金币
        if (scoreDisplay != null)
        {
            scoreDisplay.ShowGameOverResults(finalScore);
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void backMenu()
    {
        Time.timeScale = 1f; // 离开场景前记得把timeScale改回来，不然下个场景也是暂停的
        SceneManager.LoadScene("SelectLevel");
    }
}