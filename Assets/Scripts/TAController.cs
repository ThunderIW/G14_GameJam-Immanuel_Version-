using UnityEngine;

public class TAController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionDistance = 5f;
    [SerializeField] private LayerMask detectionMask; // Set this to include only the "Player" layer
    [SerializeField] private Transform raycastOrigin; // Recommended to place near "eyes"

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.left; // TA starts moving left
    private bool isPlayerInSight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        DetectPlayer();

        if (!isPlayerInSight)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
        Debug.DrawRay(raycastOrigin.position, direction * detectionDistance, Color.red);

    }
    

    private void DetectPlayer()
    {
        if (raycastOrigin == null || player == null) return;

        Vector2 origin = raycastOrigin.position;
        Vector2 target = player.position;
        Vector2 dirToPlayer = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dirToPlayer, detectionDistance, detectionMask);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (hit.collider.GetComponentInParent<PlayerController>() != null)
            {
                Debug.Log("TA sees the Player!");
                isPlayerInSight = true;
            }
            else
            {
                Debug.Log("Hit something, but not the Player.");
                isPlayerInSight = false;
            }
        }
        else
        {
            isPlayerInSight = false;
        }
    }


    // Optional patrol logic — flip direction when hitting a TurnPoint
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TurnPoint"))
        {
            direction *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    */

    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin != null && player != null)
        {
            Gizmos.color = Color.red;
            Vector3 dirToPlayer = (player.position - raycastOrigin.position).normalized;
            Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + dirToPlayer * detectionDistance);
        }
    }
}
