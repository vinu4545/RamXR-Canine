// SkeletonPartSetup.cs
// Place inside any folder named "Editor" in your Unity project.
// e.g. Assets/Editor/SkeletonPartSetup.cs
//
// Three menu items under  Tools > Vet XR :
//   1. Setup Grabbable Part   - run on selected bone mesh(es) in the hierarchy
//   2. Setup Socket           - run on selected socket mesh(es) under the skeleton machine
//   3. Auto-Match Sockets     - matches every socket's allowedId to a part's id by name
//
// Whole batch is wrapped in a single Undo group, so one Ctrl+Z reverses everything.

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public static class SkeletonPartSetup
{
    // ---------- Configuration ----------
    private const string ColliderChildName    = "Collider";
    private const string PartDataFolder       = "Assets/ScriptableObjects/PartData/Skeletal";
    private const string EventChannelPath     = "Assets/EventChannels/PartFocusEvent.asset";
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

        EnsureFolderExists(PartDataFolder);

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Setup Grabbable Parts");

        int success = 0;
        foreach (var go in selection)
        {
            if (go == null) continue;
            if (ConvertToGrabbablePart(go, eventChannel)) success++;
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
        // Find every ModelPartDataSO in the project, indexed by id (which equals the original bone mesh name).
        var allDataGuids = AssetDatabase.FindAssets("t:ModelPartDataSO");
        var dataById = new System.Collections.Generic.Dictionary<string, ModelPartDataSO>();
        foreach (var guid in allDataGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<ModelPartDataSO>(path);
            if (data != null && !string.IsNullOrEmpty(data.id))
                dataById[data.id] = data;
        }

        // Find every socket interactor in the open scene.
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
            // Match by GameObject name. "Femur" socket matches the part whose id is "Femur".
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
    private static bool ConvertToGrabbablePart(GameObject mesh, PartFocusEnteredEventChannelSO eventChannel)
    {
        // Skip if it's already a Collider duplicate or already inside a setup parent.
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

        string boneName = mesh.name;          // becomes partName + parent name
        string originalId = mesh.name;        // captured before any rename — this is the SO's id

        // ---- 1. Create empty parent matching mesh's transform ----
        var parent = new GameObject(boneName);
        Undo.RegisterCreatedObjectUndo(parent, "Create Part Parent");

        Undo.SetTransformParent(parent.transform, mesh.transform.parent, "Reparent Parent");
        parent.transform.SetSiblingIndex(mesh.transform.GetSiblingIndex());
        parent.transform.position   = mesh.transform.position;
        parent.transform.rotation   = mesh.transform.rotation;
        parent.transform.localScale = mesh.transform.localScale; // preserve world scale

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

        // Add MeshCollider with convex on. Keep MeshRenderer + MeshFilter as-is (per your spec).
        var colliderMC = Undo.AddComponent<MeshCollider>(collider);
        colliderMC.convex = true;
        var colFilter = collider.GetComponent<MeshFilter>();
        if (colFilter != null && colFilter.sharedMesh != null)
            colliderMC.sharedMesh = colFilter.sharedMesh;

        // ---- 4. Add XRGrabInteractable on parent (auto-adds Rigidbody) ----
        var grab = Undo.AddComponent<XRGrabInteractable>(parent);
        grab.useDynamicAttach = true;

        // Configure the auto-added Rigidbody.
        var rb = parent.GetComponent<Rigidbody>();
        if (rb == null) rb = Undo.AddComponent<Rigidbody>(parent);
        Undo.RecordObject(rb, "Configure Rigidbody");
        rb.useGravity  = false;
        rb.isKinematic = true;

        // ---- 5. Add XRGeneralGrabTransformer ----
        Undo.AddComponent<XRGeneralGrabTransformer>(parent);

        // ---- 6. Create / fetch the ModelPartDataSO asset ----
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
            // Asset already exists — refresh id/partName but leave descriptionChunks alone.
            Undo.RecordObject(partData, "Update existing ModelPartDataSO");
            partData.id       = originalId;
            partData.partName = boneName;
            EditorUtility.SetDirty(partData);
        }

        // ---- 7. Add ModelPart and wire references ----
        var modelPart = Undo.AddComponent<ModelPart>(parent);
        Undo.RecordObject(modelPart, "Wire ModelPart");
        modelPart.partData          = partData;
        modelPart.focusEventChannel = eventChannel;
        modelPart.interactable      = grab;
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

    // -------------------------------------------------
    // Store mesh BEFORE removing MeshFilter
    // -------------------------------------------------
    Mesh sharedMesh = null;

    var filter = socketObj.GetComponent<MeshFilter>();
    if (filter != null)
    {
        sharedMesh = filter.sharedMesh;
    }

    // -------------------------------------------------
    // Remove MeshRenderer
    // -------------------------------------------------
    var renderer = socketObj.GetComponent<MeshRenderer>();
    if (renderer != null)
    {
        Undo.DestroyObjectImmediate(renderer);
    }

    // -------------------------------------------------
    // Remove MeshFilter
    // -------------------------------------------------
    if (filter != null)
    {
        Undo.DestroyObjectImmediate(filter);
    }

    // -------------------------------------------------
    // Add MeshCollider
    // -------------------------------------------------
    var mc = socketObj.GetComponent<MeshCollider>();
    if (mc == null)
        mc = Undo.AddComponent<MeshCollider>(socketObj);

    Undo.RecordObject(mc, "Configure MeshCollider");

    mc.convex = true;
    mc.isTrigger = true;

    if (sharedMesh != null)
        mc.sharedMesh = sharedMesh;

    // -------------------------------------------------
    // Add Socket Interactor
    // -------------------------------------------------
    Undo.AddComponent<ModelPartSocketInteractor>(socketObj);

    return true;
}

    // =================================================================
    //  Helpers
    // =================================================================
    private static void EnsureFolderExists(string assetFolderPath)
    {
        if (AssetDatabase.IsValidFolder(assetFolderPath)) return;

        var parts = assetFolderPath.Split('/');
        string current = parts[0]; // "Assets"
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