using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float Timer;
    
    public GameObject settingCanva;
    public GameObject instructionPanel;
    public GameObject rushButton;

    public void Start()
    {
        settingCanva.SetActive(false);
        if(instructionPanel != null)
        {
            instructionPanelOpen();
            rushButton.SetActive(false);
        }


    }

    public void musicCanva()
    {
        settingCanva.SetActive(true);
        Time.timeScale = 0f;
    }

    public void backToGame()
    {
        settingCanva.SetActive(false);
        Time.timeScale = 1f;
    }

    private void instructionPanelOpen()
    {
        instructionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void instructionPanelClose()
    {
        instructionPanel.SetActive(false);
        Time.timeScale = 1f;
        rushButton.SetActive(true);
    }



}
