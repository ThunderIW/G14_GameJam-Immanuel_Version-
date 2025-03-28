using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isHiding = false;
    private Vector3 lastPositionBeforeHide;
    private InteractableController currentInteractable;

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

    private void Interact()
    {
        if (isHiding)
        {
            ExitHidingSpot();
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<InteractableController>();
            if (interactable != null)
            {
                interactable.Interact(this);
                return;
            }
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