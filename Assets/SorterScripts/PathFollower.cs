using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public List<Transform> points; 
    public float moveSpeed = 2f;

    private int currentIndex = 0;

    void Update()
    {
        if (points == null || points.Count == 0) return;

        Transform target = points[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= points.Count)
            {
                
                StudentSorter sorter = GetComponent<StudentSorter>();
                if (sorter != null)
                {
                    sorter.StartSorting();
                }
                else
                {
                    Destroy(gameObject); 
                }

                enabled = false; 
            }
        }

        AvoidOthers();
    }

    public void SetPath(List<Transform> path)
    {
        points = path;
        currentIndex = 0;
    }

    void AvoidOthers()
    {
        float repelRadius = 0.6f;
        float repelForce = 1.5f;

       
        int studentLayerMask = LayerMask.GetMask("Students");
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, repelRadius, studentLayerMask);

        foreach (var col in nearby)
        {
            if (col.gameObject != gameObject)
            {
                Vector3 away = transform.position - col.transform.position;
                transform.Translate(away.normalized * repelForce * Time.deltaTime);
            }
        }
    }
}
