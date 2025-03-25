using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car settings")]
    public float accelFactor = 5.0f;
    public float brakeFactor = 1.5f;
    public float turnFactor = 4.0f;
    public float driftFactor = 0.5f;
    public float maxSpeed = 4.0f;
    public float dragFactor = 3.0f;

    float accelInput = 0;
    float steerInput = 0;
    float rotationAngle = 0;
    float fwdVelocity = 0;

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
        KillSideVelocity();
        ApplySteering();
    }

    void ApplyEngineForce()
    {
        fwdVelocity = Vector2.Dot(transform.up, carRigidbody.linearVelocity);
        Debug.Log(fwdVelocity.ToString());

        //prevent car from accelerating more if it has hit its top speed
        if (fwdVelocity > maxSpeed && accelInput > 0)
        {
            return;
        }

        //limit car to 0.75x top speed in reverse
        if (fwdVelocity < -maxSpeed * 0.75f && accelInput < 0)
        {
            return;
        }

        //if car isn't accelrating, apply bunch of drag to simulate engine braking
        if (accelInput == 0)
        {
            carRigidbody.linearDamping = dragFactor;
        }
        else
        {
            carRigidbody.linearDamping = 0;
        }

        //create force for engine
        Vector2 engineForceVector = transform.up * accelInput * accelFactor;

        //make brakes more effective than accelerating
        if (fwdVelocity > 0 && accelInput < 0)
        {
            engineForceVector = engineForceVector * brakeFactor;
        }

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

    //kill orthoagonal velocity to reduce floatiness when turning
    void KillSideVelocity()
    {
        Vector2 fwdVelo = transform.up * Vector2.Dot(carRigidbody.linearVelocity, transform.up);
        Vector2 rightVelo = transform.right * Vector2.Dot(carRigidbody.linearVelocity, transform.right);

        carRigidbody.linearVelocity = fwdVelo + rightVelo * driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steerInput = inputVector.x;
        accelInput = inputVector.y; 
    }
}
