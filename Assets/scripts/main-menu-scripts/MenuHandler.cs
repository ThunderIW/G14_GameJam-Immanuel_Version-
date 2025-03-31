using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [Header("Scene Names")]
    public string optionsScene = "Options";
    public string mainMenuScene = "main_menu";
    public string startingMiniGame = "SortingMinigame";

    public void PlayGame()
    {
        GameManager.instance.LoadSceneWithFade(startingMiniGame);
    }

    public void OpenOptions()
    {
        Debug.Log("Options menu clicked!");
        GameManager.instance.LoadSceneWithFade(optionsScene);
    }

    public void GoBackToMenu()
    {
        GameManager.instance.LoadSceneWithFade(mainMenuScene);
    }

    public void Save()
    {
        Debug.Log("Saving settings...");
        GameManager.instance.LoadSceneWithFade(mainMenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        // Play click sound via GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.audioSource?.PlayOneShot(GameManager.instance.clickSound);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Debug.Log($"Fullscreen set to: {isFullScreen}");
        Screen.fullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        Debug.Log($"Quality set to: {QualitySettings.names[qualityIndex]}");
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
