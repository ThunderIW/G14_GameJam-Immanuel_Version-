using UnityEngine;

public class ScoringZone : MonoBehaviour
{
    public string acceptedMajor; 
    public AudioClip correctSound;

    private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Triggered zone!");

    AudioSource audio = gameObject.AddComponent<AudioSource>();
    audio.clip = correctSound;
    audio.Play();
}

}
