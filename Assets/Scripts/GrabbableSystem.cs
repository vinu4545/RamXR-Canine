using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabbableSystem : MonoBehaviour
{
    [System.Serializable]
    public class GrabbableEntry
    {
        public XRGrabInteractable grabComponent;
    }

    [Header("Parent containing all grabbable objects")]
    [SerializeField] private Transform grabbablesParent;

    // [Header("Auto populated at runtime")]
    private List<GrabbableEntry> grabbables = new();

    public static GrabbableSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CacheComponents();
    }

    void CacheComponents()
    {
        grabbables.Clear();

        if (grabbablesParent == null)
        {
            Debug.LogWarning("Grabbables Parent is not assigned.");
            return;
        }

        XRGrabInteractable[] grabInteractables =
            grabbablesParent.GetComponentsInChildren<XRGrabInteractable>(true);

        foreach (var grab in grabInteractables)
        {
            if (grab == null)
                continue;

            grabbables.Add(new GrabbableEntry
            {
                grabComponent = grab
            });
        }

        Debug.Log($"Cached {grabbables.Count} grabbable objects.");
    }

    public void SetEnabled(bool enabled)
    {
        Debug.Log($"GrabbableSystem: Set to {enabled}");

        foreach (var entry in grabbables)
        {
            if (entry.grabComponent == null)
                continue;

            entry.grabComponent.enabled = enabled;
        }
    }
}