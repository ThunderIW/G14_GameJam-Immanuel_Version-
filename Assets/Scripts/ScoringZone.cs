using UnityEngine;

public class ScoringZone : MonoBehaviour
{
    public string acceptedMajor;         
    public AudioClip correctSound;       
    public AudioClip incorrectSound;     

    private void OnTriggerEnter2D(Collider2D other)
{
    if (SortGameManager.Instance.hasFailed) return; 

    StudentIdentity student = other.GetComponent<StudentIdentity>();
    if (student == null) return;

    if (student.major == acceptedMajor)
    {
        
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = correctSound;
        audio.Play();

        Destroy(other.gameObject);
    }
    else
    {
        
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = incorrectSound;
        audio.Play();

        SortGameManager.Instance.Fail();
    }
}

}
