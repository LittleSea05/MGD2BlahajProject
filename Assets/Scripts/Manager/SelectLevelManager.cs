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

        public void MainMenu()
    {

        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene("MainMenu");
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

}
