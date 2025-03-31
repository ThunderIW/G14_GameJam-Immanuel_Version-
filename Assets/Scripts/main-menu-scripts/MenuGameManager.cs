using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManagerMenu : MonoBehaviour
{
    public static GameManagerMenu instance;
    private StudentSpawner spawner;
    private TAController[] controllers;
    private PlayerController playerController;
    private CampusGameManager manager;

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

    [Header("Difficulty Scaling")]
    [Header("Sorting MiniGame settings")]
    [SerializeField] private float spawnIntervalStart = 1f;
    [SerializeField] private float spawnIntervalDecreasePerLevel = 0.05f;
    [SerializeField] private float spawnIntervalMin = 0.3f;

    [SerializeField] private float moveSpeedStart = 3f; // StudentSpawner move speed
    [SerializeField] private float moveSpeedIncreasePerLevel = 0.5f;


    [Header("Avoid the TA MiniGame settings")]
    [SerializeField] private float taDetectionStart = 0.7f;
    [SerializeField] private float taDetectionIncreasePerLevel = 0.05f;

    [SerializeField] private float playerSpeedStart = 0.5f;
    [SerializeField] private float playerSpeedIncreasePerLevel = 0.05f;

    [Header("CampusDriveAround MiniGame settings")]
    [SerializeField] private float campusTimerStart = 60f;
    [SerializeField] private float campusTimerDecreasePerLevel = 10f;
    [SerializeField] private float campusTimerMin = 15f;

    [SerializeField] private int campusClassesStart = 3;
    [SerializeField] private int campusClassesIncreasePerLoop = 1;
    [SerializeField] private int campusClassesMax = 6;

    


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
        spawner = FindFirstObjectByType<StudentSpawner>();
        controllers = FindObjectsByType<TAController>(FindObjectsSortMode.None);
        playerController = FindFirstObjectByType<PlayerController>();
        manager = FindFirstObjectByType<CampusGameManager>();

        StartCoroutine(FadeInRoutine());
        UpdateLevelText();

        if (heartManager == null)
        {
            heartManager = FindFirstObjectByType<HeartManager>();
        }

        heartManager?.UpdateHearts(currentLives);

        AdjustDifficulty();
    }

    private void AdjustDifficulty()
    {
        AdjustSpawnerDifficulty();
        AdjustTADifficulty();
        AdjustPlayerDifficulty();
        AdjustCampusGameDifficulty();


    }

    private void AdjustSpawnerDifficulty()
    {
        if (spawner != null)
        {
            float newSpawnInterval = Mathf.Max(spawnIntervalMin, spawnIntervalStart - (currentLevelIndex * spawnIntervalDecreasePerLevel));
            float newMoveSpeed = moveSpeedStart + (currentLevelIndex * moveSpeedIncreasePerLevel);

            spawner.spawnInterval = newSpawnInterval;
            spawner.moveSpeed = newMoveSpeed;

            Debug.Log($"[DIFFICULTY] Spawner — Interval: {newSpawnInterval}, Speed: {newMoveSpeed}");
        }
    }

    private void AdjustTADifficulty()
    {
        if (controllers != null)
        {
            float newDetection = taDetectionStart + (currentLevelIndex * taDetectionIncreasePerLevel);

            foreach (var ta in controllers)
            {
                ta.detectionDistance = newDetection;
            }

            Debug.Log($"[DIFFICULTY] TA Detection Distance: {newDetection}");
        }
    }

    private void AdjustPlayerDifficulty()
    {
        if (playerController != null)
        {
            float newPlayerSpeed = playerSpeedStart + (currentLevelIndex * playerSpeedIncreasePerLevel);
            playerController.moveSpeed = newPlayerSpeed;

            Debug.Log($"[DIFFICULTY] Player Speed: {newPlayerSpeed}");
        }
    }

    private void AdjustCampusGameDifficulty()
    {
        if (manager != null)
        {
            float newTimer = Mathf.Max(campusTimerMin, campusTimerStart - (currentLevelIndex * campusTimerDecreasePerLevel));
            int newTotalClasses = Mathf.Min(campusClassesMax, campusClassesStart + ((loopCount - 1) * campusClassesIncreasePerLoop));

            manager.initialTimerDuration = newTimer;
            manager.totalClasses = newTotalClasses;

            // Apply to the active state too:
            manager.ResetTimer(); // Resets currentTime to the new initial value
            Debug.Log($"[DIFFICULTY] Campus — Timer: {newTimer}, Total Classes: {newTotalClasses}");
        }
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
                AnimateLevelText();
                Debug.Log("LevelText updated: LEVEL " + (currentLevelIndex + 1));
            }
        }
    }
    private void AnimateLevelText()
    {
        if (levelText == null) return;
        DOTween.Kill(levelText.transform);
        levelText.transform.localScale = Vector3.one;
        levelText.alpha = 1f;
        levelText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 5, 0.5f);

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
