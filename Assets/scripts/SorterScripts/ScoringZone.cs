using UnityEngine;

public class ScoringZone : MonoBehaviour
{
    public string acceptedMajor;         
    public AudioClip correctSound;       
    public AudioClip incorrectSound;
    [SerializeField, Range(0f, 1f)] float audioVolume = 0.75f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (SortGameManager.Instance.hasFailed) return;

        StudentIdentity student = other.GetComponent<StudentIdentity>();
        if (student == null) return;

        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.volume = audioVolume;

        if (student.major == acceptedMajor)
        {
            audio.clip = correctSound;
            audio.Play();

            SortGameManager.Instance.StudentSorted(); 
            

            Destroy(other.gameObject);
        }
        else
        {
            audio.clip = incorrectSound;
            audio.Play();

            SortGameManager.Instance.ShowFeedback("Wrong class!");
            SortGameManager.Instance.Fail();
        }
    }
}
