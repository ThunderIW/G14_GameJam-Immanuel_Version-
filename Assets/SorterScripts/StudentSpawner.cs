using UnityEngine;
using System.Collections.Generic;


public class StudentSpawner : MonoBehaviour
{
    public GameObject artStudentPrefab;
    public GameObject scienceStudentPrefab;
    public GameObject engineeringStudentPrefab;

    public Transform topLeftSpawn;
    public Transform topMiddleSpawn;
    public Transform topRightSpawn;

    public List<Transform> topLeftWaypoints;
    public List<Transform> topMiddleWaypoints;
    public List<Transform> topRightWaypoints;



    public float spawnInterval = 1f;
    public float moveSpeed = 2f;

    private float timer;

    void Start()
    {
    enabled = false;
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnStudent();
            timer = 0f;
        }
    }

    void SpawnStudent()
    {
        
        Transform[] spawnPoints = { topLeftSpawn, topMiddleSpawn, topRightSpawn };
        Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        
        GameObject prefabToSpawn;
        float rand = Random.value;
        if (rand < 0.33f)
            prefabToSpawn = artStudentPrefab;
        else if (rand < 0.66f)
            prefabToSpawn = scienceStudentPrefab;
        else
            prefabToSpawn = engineeringStudentPrefab;

        GameObject student = Instantiate(prefabToSpawn, chosenSpawn.position, Quaternion.identity);

PathFollower follower = student.AddComponent<PathFollower>();
follower.moveSpeed = moveSpeed;

if (chosenSpawn == topLeftSpawn)
{
    follower.points = new List<Transform>(topLeftWaypoints);
}
else if (chosenSpawn == topMiddleSpawn)
{
    follower.points = new List<Transform>(topMiddleWaypoints);
}
else if (chosenSpawn == topRightSpawn)
{
    follower.points = new List<Transform>(topRightWaypoints);
}




        
    }
}
