using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicTrigger : MonoBehaviour
{
    public AudioClip sceneMusic;

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log(currentScene);
        if (AudioManager.instance != null && sceneMusic)
        {
            bool shouldLoop = true;

            if (currentScene == "GameOver")
            {
                Debug.Log("Game Over scene — setting AudioSource.loop = false");
                shouldLoop = false;

                // Force override the AudioSource's loop flag
                AudioManager.instance.musicSource.loop = false;
            }
            else
            {
                AudioManager.instance.musicSource.loop = true;
            }

            AudioManager.instance.PlayMusic(sceneMusic, shouldLoop);
        }
    }
}
