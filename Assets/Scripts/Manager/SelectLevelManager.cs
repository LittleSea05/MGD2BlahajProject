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
        SceneManager.LoadScene("Level1");
    }

        public void Level2()
    {
        SceneManager.LoadScene("Level2");
    }

        public void Level3()
    {
        SceneManager.LoadScene("Level3");
    }

        public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
