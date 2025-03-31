using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("spawn point tag")]
    [SerializeField] private string spawnPointTag = "PlayerSpawn";

    [Tooltip("player gameObject")]
    [SerializeField] private GameObject playerPrefab;

    [Tooltip("player reference in scene (if already exists)")]
    [SerializeField] private GameObject playerInstance;

    [Header("Spawn Behavior")]
    [Tooltip("Teleport existing player if false")]
    [SerializeField] private bool spawnNewPlayer = false;

    [Tooltip("spawn point offset")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0, 0);

    [SerializeField] private bool randomizeInitialSpawn = true;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private List<Transform> spawnPoints = new List<Transform>();
    private Transform lastGoal;

    private void Awake()
    {
        FindAllSpawnPoints();

        // automatically get player reference if not set and exists in scene
        if (playerInstance == null)
        {
            playerInstance = GameObject.FindGameObjectWithTag("ccPlayer");
        }

        if (randomizeInitialSpawn && spawnPoints.Count > 0)
        {
            SpawnPlayerAtRandomPoint();
        }
    }

    private void FindAllSpawnPoints()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(spawnPointTag);
        spawnPoints = spawnPointObjects.Select(obj => obj.transform).ToList();

        if (showDebug)
        {
            Debug.Log($"Found {spawnPoints.Count} spawn points:");
            foreach (var spawnPoint in spawnPoints)
            {
                Debug.Log($"- {spawnPoint.name} at {spawnPoint.position}", spawnPoint);
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning($"No spawn points with tag '{spawnPointTag}' found in scene!");
        }
    }

    public void SpawnPlayerAtRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Cannot spawn player: No spawn points available!");
            return;
        }

        // select a random spawn point, preferably different from the last one
        Transform spawnPoint;
        if (spawnPoints.Count > 1 && lastGoal != null)
        {
            List<Transform> availablePoints = spawnPoints.Where(p => p != lastGoal).ToList();
            spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
        }
        else
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        lastGoal = spawnPoint;

        if (spawnNewPlayer && playerPrefab != null)
        {
            // destroy existing player if there is one
            if (playerInstance != null)
            {
                Destroy(playerInstance);
            }

            // instantiate new player
            playerInstance = Instantiate(
                playerPrefab,
                spawnPoint.position + spawnOffset,
                spawnPoint.rotation
            );

            if (showDebug) Debug.Log($"Spawned new player at {spawnPoint.name}", spawnPoint);
        }
        else if (playerInstance != null)
        {
            // tp existing player
            playerInstance.transform.position = spawnPoint.position + spawnOffset;
            playerInstance.transform.rotation = spawnPoint.rotation;

            // reset velocity
            Rigidbody2D rb2d = playerInstance.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
            }

            if (showDebug) Debug.Log($"Teleported player to {spawnPoint.name}", spawnPoint);
        }
        else
        {
            Debug.LogError("Cannot spawn player: No player instance or prefab assigned!");
        }
    }

    public void RespawnPlayerAtLastGoal(GameObject lastGoalObj)
    {
        if (lastGoalObj != null && playerInstance != null)
        {
            lastGoal = lastGoalObj.GetComponent<Transform>();
            playerInstance.transform.position = lastGoal.position + spawnOffset;
            playerInstance.transform.rotation = lastGoal.rotation;

            if (showDebug) Debug.Log($"Respawned player at last goal: {lastGoal.name}", lastGoal);
        }
        else
        {
            SpawnPlayerAtRandomPoint();
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public void RefreshSpawnPoints()
    {
        spawnPoints.Clear();
        FindAllSpawnPoints();
    }
}