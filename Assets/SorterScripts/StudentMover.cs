using UnityEngine;

public class StudentMover : MonoBehaviour
{
    public float speed = 2f;
    public float decisionY = 0f; 
    private bool hasDecided = false;

    void Update()
    {
        if (!hasDecided)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            if (transform.position.y <= decisionY)
            {
                DecideDirection();
            }
        }
    }

    void DecideDirection()
    {
        hasDecided = true;

        if (Input.GetKey(KeyCode.A))
        {
            
            StartCoroutine(MoveHorizontally(Vector3.left));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            
            StartCoroutine(MoveHorizontally(Vector3.right));
        }
        else
        {
            
            StartCoroutine(ContinueFalling());
        }
    }

    System.Collections.IEnumerator MoveHorizontally(Vector3 horizontalDirection)
    {
        float horizontalTargetX = transform.position.x + (horizontalDirection.x * 10f); 

        while (Mathf.Abs(transform.position.x - horizontalTargetX) > 0.1f)
        {
            transform.Translate(horizontalDirection * speed * Time.deltaTime);
            yield return null;
        }

        
        yield break;
    }

    System.Collections.IEnumerator ContinueFalling()
    {
        while (true)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            yield return null;
        }
    }
}
