using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float Timer;
    private bool endGame=false;
    public GameObject settingCanva;

    public void Start()
    {
        settingCanva.SetActive(false);
        Time.timeScale = 1f;
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



}
