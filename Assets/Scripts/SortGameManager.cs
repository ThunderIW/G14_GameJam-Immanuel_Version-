using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SortGameManager : MonoBehaviour
{
    public static SortGameManager Instance;

    public float resetDelay = 2f;
    public AudioClip failSound;
    public bool hasFailed = false;

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

    public void Fail()
    {
        if (hasFailed) return;

        hasFailed = true;

        StudentSpawner spawner = FindAnyObjectByType<StudentSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }

        CameraShake.Shake(0.5f, 0.5f);

        StartCoroutine(ResetGame());
    }

    IEnumerator ResetGame()
    {
        if (failSound != null)
        {
            AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position);
        }

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

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
