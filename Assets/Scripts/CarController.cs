using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car settings")]
    public float accelFactor = 30.0f;
    public float turnFactor = 3.5f;

    float accelInput = 0;
    float steerInput = 0;
    float rotationAngle = 0;

    Rigidbody2D carRigidbody;

    void Awake()
    {
        carRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
       
    }

    private void Update()
    {
 
    }

    void FixedUpdate()
    {
        ApplyEngineForce();

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        //create force for engine
        Vector2 engineForceVector = transform.up * accelInput * accelFactor;

        //apply force to car
        carRigidbody.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        //change car rotation based on input
        rotationAngle -= steerInput * turnFactor;

        //apply rotation to car
        carRigidbody.MoveRotation(rotationAngle);
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steerInput = inputVector.x;
        accelInput = inputVector.y; 
    }
}
