using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float Timer;
    
    public GameObject settingCanva;
    public GameObject instructionPanel;

    public void Start()
    {
        settingCanva.SetActive(false);
        if(instructionPanel != null)
        {
            instructionPanelOpen();
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
    }



}
