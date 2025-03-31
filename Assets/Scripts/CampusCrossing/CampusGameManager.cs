using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CampusGameManager : MonoBehaviour
{
    [Header("Goal Settings")]
    [Tooltip("goal tag")]
    public string goalTag = "ccGoal";

    [Tooltip("Current goal")]
    [SerializeField] private GameObject currentActiveGoal;
    private GameObject CurrentActiveGoal => currentActiveGoal;
    private Goal goal;

    [Header("Spawn Settings")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private bool respawnOnGoalReached = true;

    [Header("Behavior Settings")]
    public bool autoSelectNewGoal = true;
    public float minGoalChangeDelay = 1f;
    public bool hideInactiveGoals = true;
    public bool disableInactiveInteractions = true;

    [Header("Debug")]
    public bool showDebug = true;

    private List<GameObject> allGoals = new List<GameObject>();
    [SerializeField] private int goalsHit = 0;
    private void Awake()
    {
        FindAllGoalsInScene();
        if (allGoals.Count == 0)
        {
            Debug.LogWarning($"No GameObjects with tag '{goalTag}' found in scene!");
            return;
        }

        if (spawnManager == null)
        {
            spawnManager = FindAnyObjectByType<SpawnManager>();
            if (spawnManager == null && showDebug)
            {
                Debug.LogWarning("No SpawnManager found in scene. Player spawning will not work.");
            }
        }

        InitializeAllGoals();
        SelectRandomGoal();
    }

    private void FindAllGoalsInScene()
    {
        allGoals = GameObject.FindGameObjectsWithTag(goalTag).ToList();

        if (showDebug)
        {
            Debug.Log($"Found {allGoals.Count} goals in scene:");
            foreach (var goal in allGoals)
            {
                Debug.Log($"- {goal.name}", goal);
            }
        }
    }

    private void InitializeAllGoals()
    {
        foreach (var goalObj in allGoals)
        {
            // subscribe to ongoalreached event
            Goal goalComponent = goalObj.GetComponent<Goal>();
            if (goalComponent != null)
            {
                // unsubscribe first to prevent dupes
                goalComponent.onGoalEnter.RemoveListener(OnGoalReached);
                goalComponent.onGoalEnter.AddListener(OnGoalReached);
            }
            SetGoalState(goalObj, false);
        }
    }

    public void SelectRandomGoal()
    {
        if (allGoals.Count == 0) return;

        // disable current goal
        if (currentActiveGoal != null)
        {
            SetGoalState(currentActiveGoal, false);
        }

        // select new goal, excluding the current one
        var availableGoals = allGoals.Where(g => g != currentActiveGoal).ToList();
        if (availableGoals.Count == 0) availableGoals = allGoals;

        currentActiveGoal = availableGoals[Random.Range(0, availableGoals.Count)];
        SetGoalState(currentActiveGoal, true);


        if (showDebug) Debug.Log($"New goal selected: {currentActiveGoal.name}", currentActiveGoal);
    }

    public void OnGoalReached()
    {
        goalsHit++;

        if (showDebug) Debug.Log($"Goal reached: {currentActiveGoal.name}", currentActiveGoal);
        if (autoSelectNewGoal) SelectRandomGoal();

        // respawn player at random point if enabled
        if (respawnOnGoalReached && spawnManager != null)
        {
            spawnManager.SpawnPlayerAtRandomPoint();
        }
    }

    private void SetGoalState(GameObject goal, bool active)
    {
        if (goal == null) return;

        goal.SetActive(active);
    }
    public void RefreshGoalList()
    {
        FindAllGoalsInScene();
        InitializeAllGoals();

        // if current goal was destroyed, select new one
        if (currentActiveGoal == null || !allGoals.Contains(currentActiveGoal))
        {
            SelectRandomGoal();
        }
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && showDebug == true)
        {
            SelectRandomGoal();
        }

        if (Input.GetKeyUp(KeyCode.R) && spawnManager != null && showDebug)
        {
            spawnManager.SpawnPlayerAtRandomPoint();
        }
    }
}
