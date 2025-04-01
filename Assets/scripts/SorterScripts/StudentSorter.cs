using System.Collections;
using UnityEngine;

public class StudentSorter : MonoBehaviour
{
    public float speed = 2f;
    public float delayAfterPath = 0.1f;
    private Coroutine moveCoroutine;

    public void StartSorting()
    {
        moveCoroutine = StartCoroutine(SortDirection());
    }

    IEnumerator SortDirection()
    {
        yield return new WaitForSeconds(delayAfterPath);

        Vector3 direction;

        if (Input.GetKey(KeyCode.A))
        {
            direction = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = Vector3.right;
        }
        else
        {
            direction = Vector3.down;
        }

        while (true)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
}
