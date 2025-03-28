using UnityEngine;

public class InteractableController : MonoBehaviour
{
    public enum Type { Door, Locker }
    public Type interactableType;

    public Transform hidePoint;

    [Header("Visuals")]
    public GameObject model; // assign object here
    private SpriteRenderer sr;

    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openedSprite;
    public Sprite hidingSprite;

    private void Awake()
    {
        if (model != null)
            sr = model.GetComponent<SpriteRenderer>();

        sr.sprite = closedSprite;
    }

    public void Interact(PlayerController player)
    {
        player.EnterHidingSpot(this);
    }

    public void ShowOpenedFrame()
    {
        sr.sprite = openedSprite;
    }

    public void ShowHidingSprite()
    {
        sr.sprite = hidingSprite;
    }

    public void ShowClosed()
    {
        sr.sprite = closedSprite;
    }
}
