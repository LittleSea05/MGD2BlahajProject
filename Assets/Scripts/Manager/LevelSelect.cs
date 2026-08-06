using UnityEngine;
using DG.Tweening;

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

    public GameObject InfoPanel;

    void Start()
    {
        shark.position = levelPoints[0].position;

    PreviousButton.SetActive(false);
    NextButton.SetActive(true);

    ShowPanel(InfoPanel);     // 只播放一次动画

    UpdateLevelButtons();     // 显示第一个关卡资讯

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
        }
        else
        {
             LevelDescription[i].SetActive(false);
        }
    }
}

public void ShowLevelButton(GameObject obj)
{
    obj.SetActive(true);

    RectTransform rect = obj.GetComponent<RectTransform>();

    rect.localScale = Vector3.one * 0.8f;

    rect.DOScale(1f, 0.25f)
        .SetEase(Ease.OutBack);
}

public void ShowPanel(GameObject panel)
{
    panel.SetActive(true);

    RectTransform rect = panel.GetComponent<RectTransform>();

    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -10f);
    rect.localScale = Vector3.one * 0.8f;

    Sequence seq = DOTween.Sequence();

    seq.Append(
        rect.DOAnchorPosY(0, 0.40f)
            .SetEase(Ease.OutBack));

    seq.Join(
        rect.DOScale(1f, 0.40f)
            .SetEase(Ease.OutBack));
}

}