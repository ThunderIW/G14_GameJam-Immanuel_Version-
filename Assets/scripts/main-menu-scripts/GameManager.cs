using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scene Transition")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Lives UI")]
    public HeartManager heartManager;

    [Header("Game State")]
    public int startingLives = 3;
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
            SceneManager.sceneLoaded += OnSceneLoaded; // auto fade-in after any scene
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
    }

    // ------------------- LIVES & LEVEL FLOW -------------------

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
            //RestartLevel();
            AdvanceLevel();
        }
    }

    public void ResetGame()
    {
        currentLives = startingLives;
        currentLevelIndex = 0;
        LoadNextLevel();
    }

    public void AdvanceLevel()
    {
       

        currentLevelIndex++;

        if (currentLevelIndex < levelSceneNames.Length)
        {
            Debug.Log("Attempting to load level: " + levelSceneNames[currentLevelIndex]);
            LoadNextLevel();
        }
        else
        {
            Debug.Log("All levels complete!");
            LoadGameOver(); // Or load win screen
        }
    }

    public void LoadNextLevel()
    {
        if (currentLevelIndex < levelSceneNames.Length)
        {
            Debug.Log("Attempting to load level: " + levelSceneNames[currentLevelIndex]);
            LoadSceneWithFade(levelSceneNames[currentLevelIndex]);
        }
        else
        {
            Debug.LogWarning("No more levels to load.");
        }
    }

    public void RestartLevel()
    {
        LoadSceneWithFade(SceneManager.GetActiveScene().name);
    }

    public void LoadGameOver()
    {
        LoadSceneWithFade(gameOverSceneName);
    }

    // ------------------- FADE SYSTEM -------------------

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
            SceneManager.LoadScene(sceneName); // fallback if no fade image found
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
        yield return null; // wait 1 frame for scene to fully load

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
            GameObject fadeObj = GameObject.FindGameObjectWithTag("FadeUI"); // Or use tag if preferred
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
