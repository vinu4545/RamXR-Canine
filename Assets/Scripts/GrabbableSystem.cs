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

    public List<GrabbableEntry> grabbables;

    public static GrabbableSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        CacheComponents();
    }

    void CacheComponents()
    {
        foreach (var entry in grabbables)
        {
            // if (entry.targetObject == null) continue;

            // entry.grabComponent = entry.targetObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (entry.grabComponent == null)
            {
                Debug.LogWarning($"No XRGrabInteractable found on {entry}");
            }
        }
    }

    public void SetEnabled(bool enabled)
    {
        Debug.Log($"GrabbableSystem: Set to {enabled}");
        foreach (var entry in grabbables)
        {
            if (entry.grabComponent == null) continue;

            entry.grabComponent.enabled = enabled;
        }
    }
}