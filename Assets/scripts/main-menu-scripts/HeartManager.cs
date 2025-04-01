using UnityEngine;

public class HeartManager : MonoBehaviour
{
    private Hearts[] hearts;

    void Awake()
    {
        // Automatically find all Hearts components in children
        hearts = GetComponentsInChildren<Hearts>();
    }

    void Start()
    {
        if (GameManagerMenu.instance != null)
        {
            UpdateHearts(GameManagerMenu.instance.currentLives);
        }
    }

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                Debug.LogWarning($"Heart at index {i} is null!");
                continue;
            }

            hearts[i].setFull(i < lives);
        }
    }
}
