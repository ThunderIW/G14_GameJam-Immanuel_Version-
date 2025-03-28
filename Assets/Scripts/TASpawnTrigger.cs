using UnityEngine;

public class TASpawnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject firstTA;
    [SerializeField] private GameObject secondTA;
    [SerializeField] private GameObject triggerLine;
    //This script acts as the trigger for the next TA to spawn in the game. 
    // When triggered, a new line will be activated and sets off the next TA with that line
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TA"))
        {
            if (triggerLine != null) triggerLine.SetActive(true);
            if (firstTA != null) firstTA.SetActive(false);
            if (secondTA != null) secondTA.SetActive(true);
        }
    }
}
