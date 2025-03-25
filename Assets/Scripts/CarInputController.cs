using UnityEngine;

public class CarInputController : MonoBehaviour
{
    CarController carController;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        carController.SetInputVector(inputVector);
    }
}
