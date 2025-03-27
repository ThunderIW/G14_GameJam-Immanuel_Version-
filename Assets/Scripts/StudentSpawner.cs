using UnityEngine;

public class StudentSpawner : MonoBehaviour
{
    public GameObject artStudentPrefab;
    public GameObject scienceStudentPrefab;

    public GameObject engineeringStudentPrefab;

    public float spawnInterval = 1f;
    public float moveSpeed = 2f;

    private float timer;

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
    Vector3 spawnPosition = new Vector3(0f, 6f, 0);
    GameObject prefabToSpawn;

    float rand = Random.value;
    if (rand < 0.33f)
        prefabToSpawn = artStudentPrefab;
    else if (rand < 0.66f)
        prefabToSpawn = scienceStudentPrefab;
    else
        prefabToSpawn = engineeringStudentPrefab;

    GameObject student = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
}

}
