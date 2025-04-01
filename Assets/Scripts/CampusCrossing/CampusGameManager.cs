using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CampusGameManager : MonoBehaviour
{
    [Header("Goal Settings")]
    public string goalTag = "ccGoal";
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
    public int totalClasses = 3;

    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] public float initialTimerDuration = 60f;
    [SerializeField] private bool timerActive = false;
    [SerializeField] private string timerFormat = "0.0";
    private float currentTime;

    // event to fire when timer reaches zero
    public UnityEvent onTimerFinished = new UnityEvent();

    [Header("Player Input Control")]
    [SerializeField] private float inputDisableDuration = 3f;
    [SerializeField] private TextMeshProUGUI startText;
    private bool playerInputDisabled = false;
    private float inputDisableTimer = 0f;

    [Header("Other")]
    [SerializeField] private TextMeshProUGUI goalText;

    [Header("Debug")]
    public bool showDebug = true;


    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip goalReachedClip;
   

    private List<GameObject> allGoals = new List<GameObject>();
    [SerializeField] private int goalsHit = 0;
    [SerializeField] private string goalName;
    public string GoalName => goalName;

    private bool gameStarted = false;


    private void Start()
    {
        if (audioSource != null)
        {
            audioSource.Stop();           
            audioSource.clip = null;      
            audioSource.playOnAwake = false;
        }

        gameStarted = true;
    }

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
        InitializeTimer();
        DisablePlayerInput();
        ResetGame();
    }

    private void InitializeTimer()
    {
        // set starting timer value
        currentTime = initialTimerDuration;
        UpdateTimerDisplay();

        if (timerText == null && showDebug)
        {
            Debug.LogWarning("Timer TextMeshPro component is not assigned!");
        }
    }

    public void StartTimer()
    {
        timerActive = true;
        if (showDebug) Debug.Log("Timer started");
    }

    public void StopTimer()
    {
        timerActive = false;
        if (showDebug) Debug.Log("Timer stopped");
    }

    public void ResetTimer()
    {
        currentTime = initialTimerDuration;
        UpdateTimerDisplay();
        if (showDebug) Debug.Log("Timer reset");
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "Time before class starts: " + currentTime.ToString(timerFormat);
        }
    }

    public void DisablePlayerInput()
    {
        playerInputDisabled = true;
        inputDisableTimer = inputDisableDuration;

        // show start texting text
        if (startText != null)
        {
            startText.gameObject.SetActive(true);
        }

        StopTimer();

        if (showDebug) Debug.Log($"Player input disabled for {inputDisableDuration} seconds");
    }

    public void EnablePlayerInput()
    {
        playerInputDisabled = false;

        // hide starting text
        if (startText != null)
        {
            startText.gameObject.SetActive(false);
        }

        // unhide timer and goal text, start timer
        timerText.enabled = true;
        goalText.enabled = true;
        StartTimer();

        if (showDebug) Debug.Log("Player input enabled");
    }

    // public function to access player input on/off variable
    public bool isInputAllowed()
    {
        return !playerInputDisabled;
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
        UpdateGoalName(currentActiveGoal.name);
        goalText.text = "Get to the " + goalName + " building!";

        if (showDebug) Debug.Log($"New goal selected: {currentActiveGoal.name}", currentActiveGoal);
    }

    private void UpdateGoalName(string newGoalName)
    {
        goalName = newGoalName.ToLower();

        if (showDebug) Debug.Log($"Goal name updated to: {goalName}");
    }

    public void OnGoalReached()
    {
        goalsHit++;

        if (showDebug) Debug.Log($"Goal reached: {currentActiveGoal.name}", currentActiveGoal);

        
        if (goalsHit >= 1 && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(goalReachedClip);
        }

        // Play goal reached sound effect (no coroutine)
        if (gameStarted && audioSource != null && goalReachedClip != null)
        {
            audioSource.PlayOneShot(goalReachedClip);
        }

        // Respawn and select new goal
        if (respawnOnGoalReached && spawnManager != null)
        {
            spawnManager.RespawnPlayerAtLastGoal(currentActiveGoal);
        }

        if (autoSelectNewGoal)
        {
            SelectRandomGoal();
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

    private void ResetGame()
    {
        timerText.enabled = false;
        goalText.enabled = false;
    }

    private bool gameEnded = false;

    private void GameOver()
    {
        DisablePlayerInput();
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("You're winner!");

        if (GameManagerMenu.instance != null)
        {
            GameManagerMenu.instance.AdvanceLevel();
        }
    }

    void Update()
    {
        if (gameEnded) return;
        // update input disabling timer
        if (playerInputDisabled)
        {
            inputDisableTimer -= Time.deltaTime;

            // update start text
            if (startText != null)
            {
                startText.text = "Your next class is in the " + goalName + " building! \n Don't be late!";
            }

            if (inputDisableTimer <= 0)
            {
                EnablePlayerInput();
            }
        }

        // tick timer
        if (timerActive)
        {
            currentTime -= Time.deltaTime;

            // check if timer has hit zero
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerActive = false;

                //onTimerFinished.Invoke();
                DisablePlayerInput();
                if (GameManagerMenu.instance != null)
                {
                    GameManagerMenu.instance.OnPlayerDeath();
                }


                if (showDebug) Debug.Log("Timer finished!");
            }

            UpdateTimerDisplay();
        }

        if (goalsHit == totalClasses)
        {
            GameOver();
        }

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
