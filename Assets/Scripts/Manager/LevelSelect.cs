using UnityEngine;
using DG.Tweening;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    public Transform shark;
    public Transform[] levelPoints;

    public GameObject[] levelButtons;

    public float moveTime = 1.5f;

    int currentLevel = 0;

    public GameObject NextButton;
    public GameObject PreviousButton;

    public GameObject[] LevelDescription;
    public TextMeshProUGUI[] TreasureText;

    public GameObject InfoPanel;

    void Start()
    {
        shark.position = levelPoints[0].position;

    PreviousButton.SetActive(false);
    NextButton.SetActive(true);

    ShowPanel(InfoPanel);     

    UpdateLevelButtons();    

    }

    public void Update()
    {
        if(currentLevel>0)
        {
            PreviousButton.SetActive(true);
        }
        else
        {
            PreviousButton.SetActive(false);
        }

        if(currentLevel<3)
        {
            NextButton.SetActive(true);
        }
        else
        {
            NextButton.SetActive(false);
        }
        
        //levelButtons[currentLevel].SetActive(true);
    }


    public void NextLevel()
    {
        if(currentLevel >= levelPoints.Length-1)
            return;

        currentLevel++;

        MoveShark();
        UpdateLevelButtons();
        Debug.Log("you are at level: " + currentLevel);
    }

    public void PreviousLevel()
    {
        if(currentLevel <=0)
            return;

        currentLevel--;

        MoveShark();
        UpdateLevelButtons();

        Debug.Log("you are at level: " + currentLevel);
    }

    void MoveShark()
    {
        shark.DOMove(
            levelPoints[currentLevel].position,
            moveTime
        ).SetEase(Ease.InOutSine);

        shark.DOLookAt(
            levelPoints[currentLevel].position,
            0.3f
        );
    }

void UpdateLevelButtons()
{
    for (int i = 0; i < levelButtons.Length; i++)
    {
        if (i == currentLevel)
        {
            ShowLevelButton(levelButtons[i]);
        }
        else
        {
            levelButtons[i].SetActive(false);
        }
    }

    for (int i = 0; i < LevelDescription.Length; i++)
    {
        if (i == currentLevel)
        {
             ShowLevelButton(LevelDescription[i]);
             UpdateDescriptionText(i);
        }
        else
        {
             LevelDescription[i].SetActive(false);
        }
    }
}

public void UpdateDescriptionText(int levelIndex)
{
    if (TreasureText == null || levelIndex >= TreasureText.Length) return;
    if (TreasureText[levelIndex] == null) return;

    if (levelIndex == 0)
    {
        TreasureText[levelIndex].text = ""; 
    }
    else
    {
        int required = GameProgress.RequiredAmount(levelIndex - 1);
        int collected = GameProgress.GetTreasureAmount(levelIndex - 1);
        TreasureText[levelIndex].text = $"Treasure Needed: {collected}/{required}";
    }
}

public void ShowLevelButton(GameObject obj)
{
    obj.SetActive(true);

    RectTransform rect = obj.GetComponent<RectTransform>();

    rect.DOKill();

    rect.localScale = Vector3.one * 0.8f;

    rect.DOScale(1f, 0.3f)
        .SetEase(Ease.OutBack, 0.3f);
}

public void ShowPanel(GameObject panel)
{
    panel.SetActive(true);

    RectTransform rect = panel.GetComponent<RectTransform>();

    // 记住 Panel 原本的位置
    float targetY = rect.anchoredPosition.y;

    // 从原本位置下方 30px 开始
    float startY = targetY - 30f;

    rect.anchoredPosition = new Vector2(
        rect.anchoredPosition.x,
        startY
    );

    // 从稍微小一点开始
    rect.localScale = Vector3.one * 0.9f;

    Sequence seq = DOTween.Sequence();

    seq.Append(
        rect.DOAnchorPosY(targetY, 0.45f)
            .SetEase(Ease.OutCubic)
    );

    seq.Join(
        rect.DOScale(1f, 0.4f)
            .SetEase(Ease.OutCubic)
    );
}

}