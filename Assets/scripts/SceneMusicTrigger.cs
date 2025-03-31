using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip sceneMusic;
    void Start()
    {
        if(AudioManager.instance != null && sceneMusic)
        {
            AudioManager.instance.PlayMusic(sceneMusic);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
