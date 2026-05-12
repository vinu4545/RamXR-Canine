// using UnityEngine;
// using System.Collections.Generic;

// public class LabelSystem : MonoBehaviour
// {
//     [Header("Parent containing all model parts")]
//     [SerializeField] private Transform modelPartsParent;

//     private List<ModelPart> modelParts = new();

//     public static LabelSystem Instance { get; private set; }

//     void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         Instance = this;
//         CacheModelParts();
//     }

//     void CacheModelParts()
//     {
//         modelParts.Clear();

//         if (modelPartsParent == null)
//         {
//             Debug.LogWarning("LabelSystem: Model Parts Parent is not assigned.");
//             return;
//         }

//         ModelPart[] parts = modelPartsParent.GetComponentsInChildren<ModelPart>(true);

//         foreach (var part in parts)
//         {
//             if (part == null)
//                 continue;

//             modelParts.Add(part);
//         }

//         Debug.Log($"LabelSystem: Cached {modelParts.Count} model parts.");
//     }

//     public void SetEnabled()
//     {
//         Debug.Log($"LabelSystem: Turned Off labels for all model parts.");

//         foreach (var part in modelParts)
//         {
//             if (part == null)
//                 continue;

//             part.TurnOffLabel();
//         }
//     }
// }
using UnityEngine;
using System.Collections.Generic;

public class LabelSystem : MonoBehaviour
{
    [Header("Parent containing all model parts")]
    [SerializeField] private Transform modelPartsParent;

    private List<ModelPart> modelParts = new();

    private ModelPart currentActivePart;

    public static LabelSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CacheModelParts();
    }

    void CacheModelParts()
    {
        modelParts.Clear();

        if (modelPartsParent == null)
        {
            Debug.LogWarning("LabelSystem: Model Parts Parent not assigned.");
            return;
        }

        ModelPart[] parts = modelPartsParent.GetComponentsInChildren<ModelPart>(true);

        foreach (var part in parts)
        {
            if (part != null)
                modelParts.Add(part);
        }

        Debug.Log($"LabelSystem: Cached {modelParts.Count} model parts.");
    }

    public void ShowOnly(ModelPart targetPart)
    {
        if (targetPart == null)
            return;

        // Turn off previous label
        if (currentActivePart != null && currentActivePart != targetPart)
        {
            currentActivePart.TurnOffLabel();
        }

        // Show new label
        // targetPart.ShowLabel();

        currentActivePart = targetPart;
    }

    public void HideAll()
    {
        foreach (var part in modelParts)
        {
            if (part != null)
            {
                part.TurnOffLabel();
            }
        }

        currentActivePart = null;
    }
}