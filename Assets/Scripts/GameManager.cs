using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = 1;

        // Placeholder for loading the next game
        Debug.Log("Next game would load here.");
    }

    public void HandlePlayerCaught()
    {
        StartCoroutine(FadeAndReset());
    }

    private IEnumerator FadeAndReset()
    {
        // Fade to black
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Optionally fade back in
        yield return null; // wait one frame for scene to load

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeCanvas.alpha = t;
            yield return null;
        }
    }
}
