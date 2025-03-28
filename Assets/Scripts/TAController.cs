using UnityEngine;

public class TAController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionDistance = 5f;
    [SerializeField] private LayerMask obstacleMask;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right;
    private bool isPlayerInSight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Check line of sight
        Vector2 origin = transform.position;
        Vector2 target = player.position;
        Vector2 dirToPlayer = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dirToPlayer, detectionDistance, ~obstacleMask);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            isPlayerInSight = true;
        }
        else
        {
            isPlayerInSight = false;
        }

        // Move if player is NOT in sight
        if (!isPlayerInSight)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Optional: flip direction on trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TurnPoint")) // place invisible colliders to turn TA around
        {
            direction *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
