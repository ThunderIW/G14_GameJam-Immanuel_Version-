using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();
    public UnityEvent OnSpacePressed = new UnityEvent();
    public UnityEvent OnResetPressed = new UnityEvent();

    public UnityEvent<bool> OnLeftPressed = new UnityEvent<bool>();
    public UnityEvent<bool> OnRightPressed = new UnityEvent<bool>();

    void Update()
    {
        Vector2 input = Vector2.zero;

        
        if (Input.GetKeyDown(KeyCode.A))
            OnLeftPressed.Invoke(true);
        if (Input.GetKeyUp(KeyCode.A))
            OnLeftPressed.Invoke(false);

        
        if (Input.GetKeyDown(KeyCode.D))
            OnRightPressed.Invoke(true);
        if (Input.GetKeyUp(KeyCode.D))
            OnRightPressed.Invoke(false);

        if (Input.GetKey(KeyCode.A))
            input += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            input += Vector2.right;

        OnMove?.Invoke(input);

        if (Input.GetKeyDown(KeyCode.Space))
            OnSpacePressed.Invoke();

        if (Input.GetKeyDown(KeyCode.R))
            OnResetPressed.Invoke();
    }
}
