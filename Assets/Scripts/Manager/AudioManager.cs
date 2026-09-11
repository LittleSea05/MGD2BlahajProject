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
        public AudioClip ambientLoop;
    }

    public SceneMusic[] musicList;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Button Click")]
    public AudioClip defaultClickSfx;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string AMBIENT_VOLUME_KEY = "AmbientVolume";

    void Awake()
    {
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

        if (ambientSource != null)
            ambientSource.loop = true;

        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        float ambientVol = PlayerPrefs.GetFloat(AMBIENT_VOLUME_KEY, 1f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
        SetAmbientVolume(ambientVol);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayMusic(SceneManager.GetActiveScene().name);
        PlayAmbient(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic(scene.name);
        PlayAmbient(scene.name);
    }

    void PlayMusic(string sceneName)
    {
        if (musicSource == null) return;

        foreach (SceneMusic item in musicList)
        {
            if (item.sceneName == sceneName)
            {
                if (musicSource.clip == item.music)
                    return;

                musicSource.clip = item.music;
                musicSource.loop = true;
                musicSource.Play();

                return;
            }
        }
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClickSFX(AudioClip clip = null)
    {
        AudioClip clipToPlay = clip != null ? clip : defaultClickSfx;
        PlaySFX(clipToPlay);
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);

        if (sfxSource != null)
            sfxSource.volume = value;

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    void PlayAmbient(string sceneName)
    {
        if (ambientSource == null) return;

        foreach (SceneMusic item in musicList)
        {
            if (item.sceneName != sceneName)
                continue;

            if (item.ambientLoop != null)
            {
                if (ambientSource.clip != item.ambientLoop)
                {
                    ambientSource.clip = item.ambientLoop;
                    ambientSource.Play();
                }
            }
            else
            {
                ambientSource.Stop();
                ambientSource.clip = null;
            }

            return;
        }

        ambientSource.Stop();
        ambientSource.clip = null;
    }

    public void SetAmbientVolume(float value)
    {
        value = Mathf.Clamp01(value);

        if (ambientSource != null)
            ambientSource.volume = value;

        PlayerPrefs.SetFloat(AMBIENT_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public float GetAmbientVolume()
    {
        return PlayerPrefs.GetFloat(AMBIENT_VOLUME_KEY, 1f);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}