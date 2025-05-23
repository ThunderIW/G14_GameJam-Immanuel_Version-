using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SortGameManager : MonoBehaviour
{
    public static SortGameManager Instance;

    [Header("Fail Settings")]
    public float resetDelay = 2f;
    public AudioClip failSound;
    public bool hasFailed = false;

    [Header("UI Elements")]
    public TextMeshProUGUI studentsLeftText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI startPromptText;

    [Header("Gameplay Settings")]
    public int totalStudents = 20;
    private int studentsSorted = 0;
    private bool gameStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!gameStarted && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            StartGame();
        }

#if UNITY_EDITOR
        // TESTING ONLY: Press W to simulate all students sorted
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Debug.Log("FORCED WIN TRIGGERED");
        //    StartCoroutine(HandleSortWin()); // or just call EndGame() if you skip feedback
        //}
#endif
    }

    void StartGame()
    {
        gameStarted = true;

        if (startPromptText != null)
        {
            startPromptText.gameObject.SetActive(false);
        }

        StudentSpawner spawner = FindAnyObjectByType<StudentSpawner>();
        if (spawner != null)
        {
            spawner.enabled = true;
        }
    }

    public void Fail()
    {
        if (hasFailed) return;

        hasFailed = true;

        CameraShake.Shake(0.5f, 0.5f);

        // Disable the spawner
        StudentSpawner spawner = FindAnyObjectByType<StudentSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }

        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame()
    {
        if (failSound != null)
        {
            AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position);
        }

        // Stop all followers and clean them up
        PathFollower[] allFollowers = FindObjectsByType<PathFollower>(FindObjectsSortMode.None);
        foreach (var f in allFollowers)
        {
            if (f != null)
            {
                f.enabled = false;

                StudentSorter sorter = f.GetComponent<StudentSorter>();
                if (sorter != null)
                {
                    sorter.enabled = false;
                }

                Rigidbody2D rb = f.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }

        yield return new WaitForSeconds(resetDelay);

        foreach (var f in allFollowers)
        {
            if (f != null)
            {
                Destroy(f.gameObject);
            }
        }

        if (GameManagerMenu.instance != null)
        {
            GameManagerMenu.instance.OnPlayerDeath();
        }
    }

    private IEnumerator HandleSortWin()
    {
        ShowFeedback("Sorted!");

        // Freeze all students
        PathFollower[] allFollowers = FindObjectsByType<PathFollower>(FindObjectsSortMode.None);
        foreach (var f in allFollowers)
        {
            if (f != null)
            {
                f.enabled = false;

                StudentSorter sorter = f.GetComponent<StudentSorter>();
                if (sorter != null)
                {
                    sorter.enabled = false; // Stops movement and coroutine
                }

                Rigidbody2D rb = f.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }

        yield return new WaitForSeconds(2f);
        EndGame();
    }

    public void StudentSorted()
    {
        studentsSorted++;

        int remaining = totalStudents - studentsSorted;

        if (studentsLeftText != null)
        {
            studentsLeftText.text = $"Students Left: {remaining}";
        }

        if (studentsSorted >= totalStudents)
        {
            StartCoroutine(HandleSortWin());
        }
    }

    void EndGame()
    {
        Time.timeScale = 1f;
        Debug.Log("? SortGameManager triggered EndGame");

        if (GameManagerMenu.instance != null)
        {
            GameManagerMenu.instance.AdvanceLevel();
        }
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = new Color32(0, 31, 77, 255);
        feedbackText.alpha = 1f;

        StartCoroutine(FadeFeedback());
    }

    private IEnumerator FadeFeedback()
    {
        yield return new WaitForSeconds(2f);

        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = feedbackText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        feedbackText.text = "";
    }
}

