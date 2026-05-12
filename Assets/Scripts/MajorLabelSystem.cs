using UnityEngine;
using System.Collections.Generic;

public class MajorLabelSystem : MonoBehaviour
{
    [Header("All major labels in scene")]
    [SerializeField] private List<GameObject> labels = new();

    public static MajorLabelSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetEnabled(bool enabled)
    {
        Debug.Log($"MajorLabelSystem: Set labels to {enabled}");

        foreach (var label in labels)
        {
            if (label == null)
                continue;

            label.SetActive(enabled);
        }
    }
}