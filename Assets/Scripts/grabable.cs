// SkeletonPartSetup.cs
// Place inside any folder named "Editor" in your Unity project.
// e.g. Assets/Editor/SkeletonPartSetup.cs

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public static class SkeletonPartSetup
{
    // ---------- Configuration ----------
    private const string ColliderChildName    = "Collider";
    private const string PartDataFolder       = "Assets/ScriptableObjects/PartData/Skeletal";
    private const string EventChannelPath     = "Assets/EventChannels/PartFocusEvent.asset";
    private const string LabelsParentName     = "Labels"; // GameObject name in scene that holds all labels
    // -----------------------------------

    // =================================================================
    //  MENU 1: Setup Grabbable Part
    // =================================================================
    [MenuItem("Tools/Vet XR/1. Setup Grabbable Part", false, 10)]
    private static void SetupGrabbablePart()
    {
        var selection = Selection.gameObjects;
        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("Nothing selected",
                "Select one or more bone mesh GameObjects in the Hierarchy first.", "OK");
            return;
        }

        var eventChannel = AssetDatabase.LoadAssetAtPath<PartFocusEnteredEventChannelSO>(EventChannelPath);
        if (eventChannel == null)
        {
            EditorUtility.DisplayDialog("Event channel not found",
                $"Could not find PartFocusEnteredEventChannelSO at:\n{EventChannelPath}\n\n" +
                "Update EventChannelPath at the top of SkeletonPartSetup.cs if it lives elsewhere.",
                "OK");
            return;
        }

        // Find the Labels parent once for the whole batch.
        var labelsParent = FindLabelsParent();
        if (labelsParent == null)
        {
            Debug.LogWarning($"[SkeletonPartSetup] Could not find a GameObject named '{LabelsParentName}' in the scene. " +
                             "Label fields will be left empty.");
        }

        EnsureFolderExists(PartDataFolder);

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Grabbable Parts");

        int success = 0;
        foreach (var go in selection)
        {
            if (go == null) continue;
            if (ConvertToGrabbablePart(go, eventChannel, labelsParent)) success++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SkeletonPartSetup] Set up {success}/{selection.Length} grabbable part(s).");
    }

    [MenuItem("Tools/Vet XR/1. Setup Grabbable Part", true)]
    private static bool SetupGrabbablePartValidate() => Selection.gameObjects.Length > 0;

    // =================================================================
    //  MENU 2: Setup Socket
    // =================================================================
    [MenuItem("Tools/Vet XR/2. Setup Socket", false, 11)]
    private static void SetupSocket()
    {
        var selection = Selection.gameObjects;
        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("Nothing selected",
                "Select one or more socket mesh GameObjects under the skeleton machine.", "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Sockets");

        int success = 0;
        foreach (var go in selection)
        {
            if (go == null) continue;
            if (ConvertToSocket(go)) success++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"[SkeletonPartSetup] Set up {success}/{selection.Length} socket(s). " +
                  "Run 'Auto-Match Sockets' next to wire up allowedId fields.");
    }

    [MenuItem("Tools/Vet XR/2. Setup Socket", true)]
    private static bool SetupSocketValidate() => Selection.gameObjects.Length > 0;

    // =================================================================
    //  MENU 3: Auto-Match Sockets to Parts by name
    // =================================================================
    [MenuItem("Tools/Vet XR/3. Auto-Match Sockets to Parts", false, 12)]
    private static void AutoMatchSockets()
    {
        var allDataGuids = AssetDatabase.FindAssets("t:ModelPartDataSO");
        var dataById = new System.Collections.Generic.Dictionary<string, ModelPartDataSO>();
        foreach (var guid in allDataGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<ModelPartDataSO>(path);
            if (data != null && !string.IsNullOrEmpty(data.id))
                dataById[data.id] = data;
        }

        var sockets = Object.FindObjectsByType<ModelPartSocketInteractor>(FindObjectsSortMode.None);
        if (sockets.Length == 0)
        {
            Debug.LogWarning("[SkeletonPartSetup] No ModelPartSocketInteractor found in the open scene.");
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Auto-Match Sockets");

        int matched = 0, unmatched = 0;
        foreach (var socket in sockets)
        {
            string socketName = socket.gameObject.name;
            if (dataById.TryGetValue(socketName, out var data))
            {
                Undo.RecordObject(socket, "Set allowedId");
                socket.allowedId = data.id;
                EditorUtility.SetDirty(socket);
                matched++;
            }
            else
            {
                Debug.LogWarning($"[SkeletonPartSetup] No ModelPartDataSO has id '{socketName}' " +
                                 $"(socket: {GetHierarchyPath(socket.transform)})");
                unmatched++;
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"[SkeletonPartSetup] Auto-matched {matched} socket(s). {unmatched} unmatched.");
    }

    // =================================================================
    //  Core: Convert one mesh into a grabbable part
    // =================================================================
    private static bool ConvertToGrabbablePart(GameObject mesh, PartFocusEnteredEventChannelSO eventChannel, Transform labelsParent)
    {
        if (mesh.name == ColliderChildName)
        {
            Debug.LogWarning($"[SkeletonPartSetup] '{GetHierarchyPath(mesh.transform)}' looks like a Collider duplicate. Skipping.");
            return false;
        }
        if (mesh.GetComponent<ModelPart>() != null)
        {
            Debug.LogWarning($"[SkeletonPartSetup] '{mesh.name}' already has a ModelPart component. Skipping.");
            return false;
        }

        string boneName = mesh.name;
        string originalId = mesh.name;

        // ---- 1. Create empty parent matching mesh's transform ----
        var parent = new GameObject(boneName);
        Undo.RegisterCreatedObjectUndo(parent, "Create Part Parent");

        Undo.SetTransformParent(parent.transform, mesh.transform.parent, "Reparent Parent");
        parent.transform.SetSiblingIndex(mesh.transform.GetSiblingIndex());
        parent.transform.position   = mesh.transform.position;
        parent.transform.rotation   = mesh.transform.rotation;
        parent.transform.localScale = mesh.transform.localScale;

        // ---- 2. Move original mesh under parent, reset local transform ----
        Undo.SetTransformParent(mesh.transform, parent.transform, "Reparent Mesh");
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localRotation = Quaternion.identity;
        mesh.transform.localScale    = Vector3.one;

        // ---- 3. Duplicate the mesh as the Collider sibling ----
        var collider = Object.Instantiate(mesh, parent.transform);
        Undo.RegisterCreatedObjectUndo(collider, "Create Collider Duplicate");
        collider.name = ColliderChildName;
        collider.transform.localPosition = Vector3.zero;
        collider.transform.localRotation = Quaternion.identity;
        collider.transform.localScale    = Vector3.one;

        var colliderMC = Undo.AddComponent<MeshCollider>(collider);
        colliderMC.convex = true;
        var colFilter = collider.GetComponent<MeshFilter>();
        if (colFilter != null && colFilter.sharedMesh != null)
            colliderMC.sharedMesh = colFilter.sharedMesh;

        // ---- 4. XRGrabInteractable + Rigidbody ----
        var grab = Undo.AddComponent<XRGrabInteractable>(parent);
        grab.useDynamicAttach = true;

        var rb = parent.GetComponent<Rigidbody>();
        if (rb == null) rb = Undo.AddComponent<Rigidbody>(parent);
        Undo.RecordObject(rb, "Configure Rigidbody");
        rb.useGravity  = false;
        rb.isKinematic = true;

        // ---- 5. XRGeneralGrabTransformer ----
        Undo.AddComponent<XRGeneralGrabTransformer>(parent);

        // ---- 6. ModelPartDataSO ----
        string assetPath = $"{PartDataFolder}/{boneName}.asset";
        var partData = AssetDatabase.LoadAssetAtPath<ModelPartDataSO>(assetPath);
        if (partData == null)
        {
            partData = ScriptableObject.CreateInstance<ModelPartDataSO>();
            partData.id       = originalId;
            partData.partName = boneName;
            partData.descriptionChunks = new string[0];
            AssetDatabase.CreateAsset(partData, assetPath);
        }
        else
        {
            Undo.RecordObject(partData, "Update existing ModelPartDataSO");
            partData.id       = originalId;
            partData.partName = boneName;
            EditorUtility.SetDirty(partData);
        }

        // ---- 7. Find matching label under the Labels parent ----
        GameObject labelGO = null;
        if (labelsParent != null)
        {
            var labelT = FindDescendantByName(labelsParent, boneName);
            if (labelT != null)
            {
                labelGO = labelT.gameObject;
            }
            else
            {
                Debug.LogWarning($"[SkeletonPartSetup] No label named '{boneName}' found under '{LabelsParentName}'. " +
                                 "Label field left empty.");
            }
        }

        // ---- 8. Add ModelPart and wire references ----
        var modelPart = Undo.AddComponent<ModelPart>(parent);
        Undo.RecordObject(modelPart, "Wire ModelPart");
        modelPart.partData          = partData;
        modelPart.focusEventChannel = eventChannel;
        modelPart.interactable      = grab;
        modelPart.label             = labelGO;
        EditorUtility.SetDirty(modelPart);

        return true;
    }

    // =================================================================
    //  Core: Convert one mesh into a socket
    // =================================================================
    private static bool ConvertToSocket(GameObject socketObj)
    {
        if (socketObj.GetComponent<ModelPartSocketInteractor>() != null)
        {
            Debug.LogWarning($"[SkeletonPartSetup] '{socketObj.name}' already has a ModelPartSocketInteractor. Skipping.");
            return false;
        }

        Mesh sharedMesh = null;
        var filter = socketObj.GetComponent<MeshFilter>();
        if (filter != null) sharedMesh = filter.sharedMesh;

        var renderer = socketObj.GetComponent<MeshRenderer>();
        if (renderer != null) Undo.DestroyObjectImmediate(renderer);
        if (filter != null)   Undo.DestroyObjectImmediate(filter);

        var mc = socketObj.GetComponent<MeshCollider>();
        if (mc == null) mc = Undo.AddComponent<MeshCollider>(socketObj);
        Undo.RecordObject(mc, "Configure MeshCollider");
        mc.convex    = true;
        mc.isTrigger = true;
        if (sharedMesh != null) mc.sharedMesh = sharedMesh;

        Undo.AddComponent<ModelPartSocketInteractor>(socketObj);
        return true;
    }

    // =================================================================
    //  Helpers
    // =================================================================
    private static Transform FindLabelsParent()
    {
        var scene = SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == LabelsParentName) return root.transform;
            var found = FindDescendantByName(root.transform, LabelsParentName);
            if (found != null) return found;
        }
        return null;
    }

    private static Transform FindDescendantByName(Transform root, string name)
    {
        var all = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var t in all)
        {
            if (t.name == name) return t;
        }
        return null;
    }

    private static void EnsureFolderExists(string assetFolderPath)
    {
        if (AssetDatabase.IsValidFolder(assetFolderPath)) return;

        var parts = assetFolderPath.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static string GetHierarchyPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}