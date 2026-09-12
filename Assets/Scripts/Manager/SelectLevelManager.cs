using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelManager : MonoBehaviour
{
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

        public void Level1()
    {
        TryLoadLevel(1, "Level1");
    }

        public void Level2()
    {
        TryLoadLevel(2, "Level2");
    }

        public void Level3()
    {
        TryLoadLevel(3, "Level3");
    }

         public void Shop()
    {
        SceneManager.LoadScene("Shop");
    }

           public void Options()
    {
        SceneManager.LoadScene("Option");
    }

            public void SelectLevel()
    {
        SceneManager.LoadScene("SelectLevel");
    }

               public void Credits()
    {
        SceneManager.LoadScene("Credit");
    }

        public void MainMenu()
    {

        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene("MainMenu");

        Time.timeScale = 1f;
        Debug.Log("Time scale reset to 1");
    }

    private void TryLoadLevel(int levelIndex, string sceneName)
    {
        if (GameProgress.IsLevelUnlocked(levelIndex))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            LockStartButton.Instance.ShowWarning();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
