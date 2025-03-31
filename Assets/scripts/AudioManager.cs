using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Default Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip clickSound;

    private void Awake()
    {
        // Check if another AudioManager already exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist through scene changes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }
    }

    void Start()
    {
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        Debug.Log($"Loaded volumes: Music = {savedMusicVol}");
        SetMusicVolume(savedMusicVol);
        // If a default music clip is set, play it
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) {
            Debug.LogWarning("PlayMusic: musicSource or clip is null!");
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying) {
            Debug.Log("PlayMusic: Already playing this clip.");
            return;
        }


        Debug.Log("PlayMusic: Switching music to " + clip.name);
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }


    public void MuteAll(bool isMuted)
    {
        if (musicSource != null)
        {
            musicSource.mute=isMuted;

        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }
}
