using UnityEngine;
using UnityEngine.SceneManagement;


public class TAController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    
    [Header("Layer Masks")]
    [SerializeField] private LayerMask detectionMask; 

    [Header("Raycast")]
    [SerializeField] private Transform raycastOrigin; 

    [Header("Emote")]
    [SerializeField] private GameObject emote; 

    [Header("Tweakables")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionDistance = 5f;
    



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
    
    //Function to detect player with Raycast (constantly updated with FixedUpdate)
    private void DetectPlayer()
    {
        if (raycastOrigin == null || player == null) return;

        Vector2 origin = raycastOrigin.position;
        Vector2 target = player.position;
        Vector2 dirToPlayer = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dirToPlayer, detectionDistance, detectionMask);

        if (hit.collider != null)
        {

            if (hit.collider.GetComponentInParent<PlayerController>() != null)
            {
            isPlayerInSight = true;
            emote.SetActive(true);
            animator.Play("TA_angryleft");
            Debug.Log("Player caught! Reloading scene...");
            GameManager.Instance.HandlePlayerCaught();
            }
            else
            {
                isPlayerInSight = false;
            }
        }
        else
        {
            emote.SetActive(false);
            isPlayerInSight = false;
        }
    }

    //Debug draw raycast ray
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
