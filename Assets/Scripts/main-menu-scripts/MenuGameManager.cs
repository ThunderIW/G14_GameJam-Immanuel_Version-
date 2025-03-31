using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManagerMenu : MonoBehaviour
{
    public static GameManagerMenu instance;

    [Header("Scene Transition")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Lives UI and Level UI")]
    public HeartManager heartManager;
    public TextMeshProUGUI levelText;

    [Header("Game State")]
    public int startingLives = 3;
    public int loopCount { get; private set; } = 1;
    public int currentLives { get; private set; }
    public int currentLevelIndex { get; private set; }

    [Header("Scene Names")]
    public string gameOverSceneName = "GameOver";
    public string[] levelSceneNames;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLives = startingLives;
        currentLevelIndex = 0;
    }

    void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInRoutine());
        UpdateLevelText();

        if (heartManager == null)
        {
            heartManager = FindFirstObjectByType<HeartManager>();
        }

        heartManager?.UpdateHearts(currentLives);
    }

    public void UpdateLevelText()
    {
        GameObject levelTextObj = GameObject.FindWithTag("LevelText");

        if (levelTextObj != null)
        {
            levelText = levelTextObj.GetComponent<TextMeshProUGUI>();
            if (levelText != null)
            {
                levelText.text = "LEVEL: " + (currentLevelIndex + 1);
                Debug.Log("LevelText updated: LEVEL " + (currentLevelIndex + 1));
            }
        }
    }

    public void OnPlayerDeath()
    {
        currentLives--;

        if (heartManager == null)
        {
            heartManager = FindFirstObjectByType<HeartManager>();
        }

        heartManager?.UpdateHearts(currentLives);
        Debug.Log("Player died! Lives left: " + currentLives);

        if (currentLives <= 0)
        {
            Debug.Log("No lives left. Going to Game Over...");
            LoadGameOver();
        }
        else
        {
            Debug.Log("Advancing to next level after failure...");
            AdvanceLevel();
        }
    }

    public void ResetGame()
    {
        currentLives = startingLives;
        currentLevelIndex = 0;
        loopCount = 1;

        if (heartManager == null)
        {
            heartManager = FindFirstObjectByType<HeartManager>();
        }
        heartManager?.UpdateHearts(currentLives);

        Debug.Log("Game reset. Starting over at LEVEL 1.");
        LoadNextLevel(0);
    }

    public void AdvanceLevel()
    {
        currentLevelIndex++;
        int sceneIndex = currentLevelIndex % levelSceneNames.Length;

        if (sceneIndex == 0 && currentLevelIndex > 0)
        {
            loopCount++;
            Debug.Log("Loop complete! Starting loop #" + loopCount);
        }

        Debug.Log($"Advancing to LEVEL {currentLevelIndex + 1} (Scene: {levelSceneNames[sceneIndex]})");
        LoadNextLevel(sceneIndex);
    }

    public void LoadNextLevel(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < levelSceneNames.Length)
        {
            Debug.Log("Attempting to load level: " + levelSceneNames[sceneIndex]);
            LoadSceneWithFade(levelSceneNames[sceneIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid scene index: " + sceneIndex);
        }
    }

    public void RestartLevel()
    {
        LoadSceneWithFade(SceneManager.GetActiveScene().name);
    }

    public void LoadGameOver()
    {
        int levelReached = currentLevelIndex + 1;
        PlayerPrefs.SetInt("LastLevelReached", levelReached);
        Debug.Log($"LastLevelReached: {levelReached}");

        int previousHigh = PlayerPrefs.GetInt("HighScore", 0);
        if (levelReached > previousHigh)
        {
            PlayerPrefs.SetInt("HighScore", levelReached);
        }

        PlayerPrefs.Save();
        LoadSceneWithFade(gameOverSceneName);
    }

    public void LoadSceneWithFade(string sceneName)
    {
        TryAssignFadeImage();

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

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadeImage == null) TryAssignFadeImage();

        DOTween.Kill(fadeImage);

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        yield return fadeImage.DOFade(1f, fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(sceneName);
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        yield return null;

        TryAssignFadeImage();

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            DOTween.Kill(fadeImage);
            fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
            {
                fadeImage.raycastTarget = false;
            });
        }
        else
        {
            Debug.LogWarning("FadeImage not found — fade-in skipped.");
        }
    }

    private void TryAssignFadeImage()
    {
        if (fadeImage == null)
        {
            GameObject fadeObj = GameObject.FindGameObjectWithTag("FadeUI");
            if (fadeObj != null)
            {
                fadeImage = fadeObj.GetComponent<Image>();
                Debug.Log("FadeImage assigned dynamically.");
            }
            else
            {
                Debug.LogWarning("Could not find FadeImage in scene.");
            }
        }
    }
}