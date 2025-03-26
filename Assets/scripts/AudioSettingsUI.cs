using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Audio Slider")]
    public Slider musicSlider;
    public TextMeshProUGUI VolumeLevel;

    [Header("Sprites for volume UI")]
    public Image volumeIcon;
    public Sprite VolumeOnSprite;
    public Sprite VolumeOffSprite;

    private bool isMuted;

    void Start()
    {
        // Load mute state
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;

        // Load saved volume (default to 1)
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);

        float displayVolume = isMuted ? 0f : savedMusicVol;

        musicSlider.value = displayVolume;
        AudioManager.instance.SetMusicVolume(displayVolume);

        if (VolumeLevel != null)
            VolumeLevel.text = Mathf.RoundToInt(displayVolume * 100f) + "%";

        UpdateMuteIcon();
        AudioManager.instance.MuteAll(isMuted);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioManager.instance.MuteAll(isMuted);
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);

        if (isMuted)
        {
            musicSlider.value = 0f;
            VolumeLevel.text = "0%";
            AudioManager.instance.SetMusicVolume(0f);
        }
        else
        {
            float newVol = 0.35f;
            musicSlider.value = newVol;
            VolumeLevel.text = Mathf.RoundToInt(newVol * 100f) + "%";
            AudioManager.instance.SetMusicVolume(newVol);
            PlayerPrefs.SetFloat("MusicVolume", newVol);
        }

        PlayerPrefs.Save();
        UpdateMuteIcon();
    }

    private void UpdateMuteIcon()
    {
        if (volumeIcon != null)
            volumeIcon.sprite = isMuted ? VolumeOffSprite : VolumeOnSprite;
    }

    public void ResetAudio()
    {
        musicSlider.value = 1f;
        SetMusicVolume(1f);

        isMuted = false;
        AudioManager.instance.MuteAll(false);
        PlayerPrefs.SetInt("Muted", 0);
        PlayerPrefs.Save();

        VolumeLevel.text = "100%";
        UpdateMuteIcon();
    }

    void SetMusicVolume(float volume)
    {
        if (!isMuted)
        {
            AudioManager.instance.SetMusicVolume(volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }

        if (VolumeLevel != null)
        {
            VolumeLevel.text = Mathf.RoundToInt(volume * 100f) + "%";

            VolumeLevel.transform.DOKill();
            VolumeLevel.transform.localScale = Vector3.one;
            VolumeLevel.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 0.5f);
        }
    }

    void Update() { }
}
