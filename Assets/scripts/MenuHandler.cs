using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class MenuHandler : MonoBehaviour
{
    [Header("UI Fade Image (fullscreen)")]
    public Image fadeImage;

    [Header("Fade duration in seconds")]
    public float fadeDuration = 1f;

    [Header("sound")]
    public AudioSource audioSource;
    public AudioClip clickSound;


    [Header("Scences")]
    public string optionsSceneName = "Options";
    public string mainMenuSceneName = "main_menu";
    void Start()
    {
        // Fade in from black when menu loads
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
            {
                fadeImage.raycastTarget = false; 
            });
        }
    }

    public void PlayGame()
    {
        PlayClickSound();
        StartSceneTransition("YourGameSceneName");
    }

    public void OpenOptions()
    {
        PlayClickSound();
        StartSceneTransition(optionsSceneName);
        Debug.Log("Options menu clicked! Add your logic here.");
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Quitting game...");
        //Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    public void Save()
    {

        PlayClickSound();
        StartSceneTransition(mainMenuSceneName);
    }

    void StartSceneTransition(string sceneName)
    {
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true;
            StartCoroutine(FadeAndLoad(sceneName));



        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void PlayClickSound()
    {
        if(audioSource!= null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return fadeImage.DOFade(1, fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }
}

