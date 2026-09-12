using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    private enum UpgradeType { Speed, Weight, Hunger }
    
    [Header("Coins")]
    public ScoreDisplay scoreDisplay;

    [Header("speed")]
    public GameObject[] speedShellIcons = new GameObject[3];
    public Button speedUpgradeButton;
    public TMP_Text speedCannotUpgradeText; // 新增

    [Header("weight")]
    public GameObject[] weightShellIcons = new GameObject[3];
    public Button weightUpgradeButton;
    public TMP_Text weightCannotUpgradeText; // 新增

    [Header("hunger")]
    public GameObject[] hungerShellIcons = new GameObject[3];
    public Button hungerUpgradeButton;
    public TMP_Text hungerCannotUpgradeText; // 新增

    [Header("confirmation  ")]
    public GameObject confirmPanel;
    public TMP_Text confirmMessageText;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private UpgradeType pendingType;

    void Start()
    {
        speedUpgradeButton.onClick.AddListener(() => OpenConfirmPanel(UpgradeType.Speed));
        weightUpgradeButton.onClick.AddListener(() => OpenConfirmPanel(UpgradeType.Weight));
        hungerUpgradeButton.onClick.AddListener(() => OpenConfirmPanel(UpgradeType.Hunger));

        confirmYesButton.onClick.AddListener(OnConfirmYes);
        confirmNoButton.onClick.AddListener(CloseConfirmPanel);

        confirmPanel.SetActive(false);
        RefreshUI();
    }

    void OnEnable()
    {
        RefreshUI();
    }

    void OpenConfirmPanel(UpgradeType type)
    {
        int cost = GetNextCost(type);
        if (cost < 0) return; 

        pendingType = type;
        confirmMessageText.text = $"Upgrade{GetDisplayName(type)} need {cost} coins, do you want to upgrade?";
        confirmPanel.SetActive(true);
    }

    void OnConfirmYes()
    {
        bool success = pendingType switch
        {
            UpgradeType.Speed => PlayerUpgradeData.TryUpgradeSpeed(),
            UpgradeType.Weight => PlayerUpgradeData.TryUpgradeWeight(),
            UpgradeType.Hunger => PlayerUpgradeData.TryUpgradeHunger(),
            _ => false
        };

        if (!success)
        {
            Debug.Log("Upgrade failed: Not enough coins or already at maximum level");
        }

        CloseConfirmPanel();
        RefreshUI();
    }

    void CloseConfirmPanel()
    {
        confirmPanel.SetActive(false);
    }

    void RefreshUI()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.RefreshCoins();
        }

        int coins = ScoreManager.Instance != null ? ScoreManager.Instance.TotalCoins : 0;

        RefreshShellIcons(speedShellIcons, PlayerUpgradeData.SpeedLevel);
        RefreshShellIcons(weightShellIcons, PlayerUpgradeData.WeightLevel);
        RefreshShellIcons(hungerShellIcons, PlayerUpgradeData.HungerLevel);

        bool canSpeed = CanAffordNext(UpgradeType.Speed, coins);
        bool canWeight = CanAffordNext(UpgradeType.Weight, coins);
        bool canHunger = CanAffordNext(UpgradeType.Hunger, coins);

        speedUpgradeButton.interactable = canSpeed;
        weightUpgradeButton.interactable = canWeight;
        hungerUpgradeButton.interactable = canHunger;

        // 新增：按钮不可点时显示对应提示，可以点时隐藏
        SetCannotUpgradeText(speedCannotUpgradeText, !canSpeed);
        SetCannotUpgradeText(weightCannotUpgradeText, !canWeight);
        SetCannotUpgradeText(hungerCannotUpgradeText, !canHunger);
    }

    // 新增：统一控制提示文字显隐
    void SetCannotUpgradeText(TMP_Text text, bool show)
    {
        if (text == null) return;

        text.gameObject.SetActive(show);
        if (show)
        {
            text.text = "Cannot be level up now.";
        }
    }

    void RefreshShellIcons(GameObject[] shellIcons, int level)
    {
        for (int i = 0; i < shellIcons.Length; i++)
        {
            if (shellIcons[i] != null)
            {
                shellIcons[i].SetActive(i < level);
            }
        }
    }

    int GetNextCost(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Speed => PlayerUpgradeData.GetNextSpeedCost(),
            UpgradeType.Weight => PlayerUpgradeData.GetNextWeightCost(),
            UpgradeType.Hunger => PlayerUpgradeData.GetNextHungerCost(),
            _ => -1
        };
    }

    bool CanAffordNext(UpgradeType type, int coins)
    {
        int cost = GetNextCost(type);
        return cost >= 0 && coins >= cost;
    }

    string GetDisplayName(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Speed => "Speed",
            UpgradeType.Weight => "Weight",
            UpgradeType.Hunger => "Hunger",
            _ => ""
        };
    }
}