using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip music;
    }

    [Header("Scene Music")]
    public SceneMusic[] musicList;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        // 監聽 Scene 切換
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic(scene.name);
    }

    void PlayMusic(string sceneName)
    {
        foreach (SceneMusic item in musicList)
        {
            if (item.sceneName == sceneName)
            {
                if (audioSource.clip == item.music)
                    return;

                audioSource.clip = item.music;
                audioSource.loop = true;
                audioSource.Play();

                return;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}