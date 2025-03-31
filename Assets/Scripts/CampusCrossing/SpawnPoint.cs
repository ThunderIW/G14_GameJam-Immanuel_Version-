using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [Tooltip("Whether this spawn point is currently active")]
    [SerializeField] private bool isActive = true;
    public bool IsActive => isActive;

    public void SetActive(bool active)
    {
        isActive = active;
    }
}