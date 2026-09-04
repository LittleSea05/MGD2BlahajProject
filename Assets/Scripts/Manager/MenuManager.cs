using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //public GameObject SceneLight;
    //public GameObject Explanation;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene("MainMenu");
    }

     public void GoLevelSelection()
    {
        SceneManager.LoadScene("SelectLevel");
    }

     public void GoOptionScene()
    {
        SceneManager.LoadScene("Option");
    }

     public void GoCollection()
    {
        SceneManager.LoadScene("06ThreePoint");
    }

 //  public void ToggleLight()
   // {
   // SceneLight.SetActive(!SceneLight.activeSelf);
   // }

   // public void Descriptions()
  //  {
   // Explanation.SetActive(!Explanation.activeSelf);
   // }

    public void QuitGame()
    {
        Application.Quit();
    }
}
