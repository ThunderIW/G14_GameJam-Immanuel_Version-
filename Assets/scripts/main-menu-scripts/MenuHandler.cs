using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [Header("Scene Names")]
    public string optionsScene = "Options";
    public string mainMenuScene = "main_menu";
    public string startingMiniGame = "SortingMinigame";

    public void PlayGameFromStart()
    {
        if (GameManagerMenu.instance != null)
        {
            GameManagerMenu.instance.ResetGame(); // Ensures clean start
        }
        else
        {
            Debug.LogWarning("GameManagerMenu instance is null in PlayGame()");
        }
    }

    public void OpenOptions()
    {
        Debug.Log("Options menu clicked!");
        GameManagerMenu.instance.LoadSceneWithFade(optionsScene);
    }

    public void GoBackToMenu()
    {
        GameManagerMenu.instance.LoadSceneWithFade(mainMenuScene);
    }

    public void Save()
    {
        Debug.Log("Saving settings...");
        GameManagerMenu.instance.LoadSceneWithFade(mainMenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        // Play click sound via GameManager
        if (GameManagerMenu.instance != null)
        {
            GameManagerMenu.instance.audioSource?.PlayOneShot(GameManagerMenu.instance.clickSound);
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
    public void PlayAgain()
    {
        Debug.Log("Play Again clicked!");

        if (GameManagerMenu.instance != null)
        {
            GameManagerMenu.instance.ResetGame();
        }
        else
        {
            Debug.LogWarning("GameManagerMenu instance not found — cannot restart.");
        }
    }
}
