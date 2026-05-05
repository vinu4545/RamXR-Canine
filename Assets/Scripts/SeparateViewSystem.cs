using UnityEngine;

public class SeparateViewSystem : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO activateEvent;
    [SerializeField] private VoidEventChannelSO deactivateEvent;

    private bool isActive;

    public static SeparateViewSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of SeparateViewSystem detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetEnabled(bool enabled)
    {
        // Prevent redundant calls
        if (isActive == enabled) return;

        isActive = enabled;

        if (enabled)
        {
            activateEvent?.RaiseEvent();
        }
        else
        {
            deactivateEvent?.RaiseEvent();
        }
    }
}