using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;

    private bool isCaughtHandled = false;

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

        Debug.Log("Next game would load here.");
        GameManagerMenu.instance.AdvanceLevel();
    }

    public void HandlePlayerCaught()
    {
        if (isCaughtHandled)
        {
            Debug.Log("HandlePlayerCaught called again — skipping.");
            return;
        }

        isCaughtHandled = true;

        Debug.Log("HandlePlayerCaught triggered.");

        GameManagerMenu.instance.OnPlayerDeath();
    }

    private IEnumerator FadeAndReset()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            fadeCanvas.alpha = t;
            yield return null;
        }

        fadeCanvas.alpha = 1;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        yield return null;
        yield return new WaitForSeconds(0.5f);

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / fadeDuration;
            fadeCanvas.alpha = t;
            yield return null;
        }

        fadeCanvas.alpha = 0;
    }

    // Optional: call this when resetting game state
    public void ResetCaughtFlag()
    {
        isCaughtHandled = false;
    }
}
