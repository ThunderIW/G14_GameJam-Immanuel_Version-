using UnityEngine;

public class HeartManager : MonoBehaviour
{
    public Hearts[] hearts;

    void Start()
    {
        if (GameManagerMenu.instance != null) {
            UpdateHearts(GameManagerMenu.instance.currentLives);
        }

       
    }

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].setFull(i < lives);
        }
    }
}
