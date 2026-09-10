using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 商城UI逻辑（贝壳等级显示 + 升级确认弹窗版）。
/// 金币数字显示统一交给ScoreDisplay，这个脚本不再自己拿Text设置金额。
/// 三条升级线：速度(Speed)、体型(Weight)、生命值(Hunger)，每条都是3级。
/// </summary>
public class ShopManager : MonoBehaviour
{
    private enum UpgradeType { Speed, Weight, Hunger }
    
    [Header("Coins")]
    public ScoreDisplay scoreDisplay;

    [Header("speed")]
    public GameObject[] speedShellIcons = new GameObject[3];
    public Button speedUpgradeButton;

    [Header("weight")]
    public GameObject[] weightShellIcons = new GameObject[3];
    public Button weightUpgradeButton;

    [Header("hunger")]
    public GameObject[] hungerShellIcons = new GameObject[3];
    public Button hungerUpgradeButton;

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
        confirmMessageText.text = $"Upgrade{GetDisplayName(type)} need {cost} coins, are you sure you want to upgrade?";
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

        speedUpgradeButton.interactable = CanAffordNext(UpgradeType.Speed, coins);
        weightUpgradeButton.interactable = CanAffordNext(UpgradeType.Weight, coins);
        hungerUpgradeButton.interactable = CanAffordNext(UpgradeType.Hunger, coins);
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