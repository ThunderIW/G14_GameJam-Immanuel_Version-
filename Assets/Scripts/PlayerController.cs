using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask interactableMask;
    


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
        inputManager.OnResetPressed.AddListener(ResetPlayer);
        inputManager.OnSpacePressed.AddListener(Interact);
    }

    private void FixedUpdate()
    {
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
    }

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
            // Hide previous
            if (lastDetectedInteractable != null)
                lastDetectedInteractable.ShowPopup(false);

            // Show new
            if (current != null)
                current.ShowPopup(true);

            lastDetectedInteractable = current;
        }
    }


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

    private void HideInside()
    {
        currentInteractable.ShowHidingSprite();
        transform.position = currentInteractable.hidePoint.position;
        gameObject.SetActive(false);
    }

    public void ExitHidingSpot()
    {
        if (!isHiding || currentInteractable == null) return;

        isHiding = false;

        currentInteractable.ShowOpenedFrame();
        transform.position = lastPositionBeforeHide;
        gameObject.SetActive(true);

        Invoke(nameof(CloseAfterExit), 0.1f); // Show open frame briefly
    }

    private void CloseAfterExit()
    {
        currentInteractable.ShowClosed();
        currentInteractable = null;
    }

    private void ResetPlayer()
    {
        Debug.Log("Player Reset (R)");
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector3.zero;
    }
}