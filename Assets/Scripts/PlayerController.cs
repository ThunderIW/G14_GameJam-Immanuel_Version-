using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Animator animator;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask detectionMask; 

    [Header("Raycast")]
    [SerializeField] private Transform raycastOrigin;

    [Header("Emote")]
    [SerializeField] private GameObject emote; 

    [Header("Tweakables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float detectionDistance = 8f;

    


    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isHiding = false;
    private Vector3 lastPositionBeforeHide;
    private InteractableController currentInteractable;
    private InteractableController lastDetectedInteractable;
    private bool hasHiddenBefore = false;



    private string lastDirection = "right"; // default facing direction

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        inputManager.OnMove.AddListener(HandleMoveInput);
        inputManager.OnLeftPressed.AddListener(HandleLeftInput);
        inputManager.OnRightPressed.AddListener(HandleRightInput);
        inputManager.OnSpacePressed.AddListener(Interact);
    }

    private void FixedUpdate()
    {
        DetectTA();

        if (!isHiding)
        {
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (!hasHiddenBefore)
            HandleInteractablePopup();
        
        Debug.DrawRay(raycastOrigin.position, moveDirection * detectionDistance, Color.yellow);

    }

    //Used Raycast to check for any TA object. 
    //When triggered, the scared :o emote pops up to to prompt player to hide
    private void DetectTA()
    {
        if (raycastOrigin == null) return;

        Vector2 origin = raycastOrigin.position;
        Vector2 dir = lastDirection == "right" ? Vector2.right : Vector2.left;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, detectionDistance, detectionMask);
        foreach (var hit in hits)
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
        }
        Debug.DrawRay(origin, dir * detectionDistance, Color.yellow);

        bool sawTA = false;

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.GetComponentInParent<TAController>() != null)
            {
                sawTA = true;
                break;
            }
        }

        emote.SetActive(sawTA);
    }
    //Draw the raycast line for debug
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 dir = lastDirection == "right" ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + (Vector3)dir * detectionDistance);
        }
    }


    //Handles the appropriate anims for the player depedning on their state
    private void HandleMoveInput(Vector2 input)
    {
        moveDirection = input.normalized;

        bool isWalking = Mathf.Abs(moveDirection.x) > 0;
        animator.SetBool("isWalking", isWalking);

        if (!isWalking)
        {
            if (lastDirection == "left")
                animator.Play("mc_idleleft");
            else
                animator.Play("mc_idleright");
        }
    }

    private void HandleLeftInput(bool isPressed)
    {
        if (isPressed && !isHiding)
        {
            animator.Play("mc_walkleft");
            lastDirection = "left";
        }
    }

    private void HandleRightInput(bool isPressed)
    {
        if (isPressed && !isHiding)
        {
            animator.Play("mc_walkright");
            lastDirection = "right";
        }
    }

    //Handles the hiding mechanic, when the player presses spacebar, they can hide in a classroom or locker
    private void Interact()
    {
        if (isHiding)
        {
            ExitHidingSpot();
            return;
        }

        Vector2 origin = transform.position;
        Vector2 dir = lastDirection == "left" ? Vector2.left : Vector2.right;
        float interactRange = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(origin + dir * 0.1f, dir, interactRange, interactableMask);
        Debug.DrawRay(origin + dir * 0.1f, dir * interactRange, Color.green, 1f);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);
            var interactable = hit.collider.GetComponent<InteractableController>();
            if (interactable != null)
            {
                interactable.Interact(this);
            }
            else
            {
                Debug.Log("Hit something, but not interactable: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("No interactable in direction: " + dir);
        }
    }
    //Spacebar popup when player is near door or locker
    private void HandleInteractablePopup()
    {
        Vector2 origin = transform.position;
        Vector2 dir = lastDirection == "left" ? Vector2.left : Vector2.right;
        float interactRange = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(origin + dir * 0.1f, dir, interactRange, interactableMask);

        InteractableController current = null;
        if (hit.collider != null)
        {
            current = hit.collider.GetComponent<InteractableController>();
        }

        if (current != lastDetectedInteractable)
        {
            if (lastDetectedInteractable != null)
                lastDetectedInteractable.ShowPopup(false);

            if (current != null)
                current.ShowPopup(true);

            lastDetectedInteractable = current;
        }
    }

    
    //Script to open the door/locker and show player hiding
    public void EnterHidingSpot(InteractableController interactable)
    {
        if (isHiding) return;

        isHiding = true;
        lastPositionBeforeHide = transform.position;
        currentInteractable = interactable;

        interactable.ShowOpenedFrame();
        Invoke(nameof(HideInside), 0.1f); // Small delay for open frame
        if (!hasHiddenBefore)
        {
            hasHiddenBefore = true;
            currentInteractable.HidePopup(); // Disable popup after first hide
        }

    }

    //Show the player hiding in room/locker
    private void HideInside()
    {
        currentInteractable.ShowHidingSprite();
        transform.position = currentInteractable.hidePoint.position;
        gameObject.SetActive(false);
    }

    //Exit the hiding spot 
    public void ExitHidingSpot()
    {
        if (!isHiding || currentInteractable == null) return;

        isHiding = false;

        currentInteractable.ShowOpenedFrame();
        transform.position = lastPositionBeforeHide;
        gameObject.SetActive(true);

        Invoke(nameof(CloseAfterExit), 0.1f); // Show open frame briefly
    }

    //Close the door and return to normal sprite
    private void CloseAfterExit()
    {
        currentInteractable.ShowClosed();
        currentInteractable = null;
    }
}