using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    [Tooltip("player gameObject")]
    [SerializeField] private string playerTag = "ccPlayer";

    public UnityEvent onGoalEnter = new UnityEvent();

    // fire event when goal is entered
    private void OnTriggerEnter2D(Collider2D colliderObject)
    {
        if (ShouldDetectCollision(colliderObject.gameObject))
        {
            Debug.Log($"{colliderObject.gameObject.name} collided with {gameObject.name}");
            onGoalEnter.Invoke();
        }
    }

    // helper method to make sure player was what entered goal
    private bool ShouldDetectCollision(GameObject player)
    {
        if (player != null && player.tag == playerTag)
        {
            return true;
        }

        return false;
    }
}
