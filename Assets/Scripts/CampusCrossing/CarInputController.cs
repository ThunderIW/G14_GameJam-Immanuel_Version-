using System;
using UnityEngine;

public class CarInputController : MonoBehaviour
{
    CarController carController;
    CampusGameManager gameManager;

    private void Awake()
    {
        carController = GetComponent<CarController>();
        gameManager = FindAnyObjectByType<CampusGameManager>();
    }
    void Update()
    {
        if (gameManager.isInputAllowed())
        {
            Vector2 inputVector = Vector2.zero;

            inputVector.x = Input.GetAxis("Horizontal");
            inputVector.y = Input.GetAxis("Vertical");

            carController.SetInputVector(inputVector);
        }
    }
}
