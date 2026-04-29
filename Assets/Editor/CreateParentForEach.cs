using UnityEngine;
using UnityEditor;
using System.Linq;

public class CreateParentForEach
{
    [MenuItem("GameObject/Create Empty Parent For Each Selected %#g")]
    static void CreateParent()
    {
        // Get only top-level selected objects
        var selected = Selection.gameObjects
            .Where(obj => obj.transform.parent == null ||
                         !Selection.transforms.Contains(obj.transform.parent))
            .ToArray();

        foreach (GameObject obj in selected)
        {
            GameObject parent = new GameObject(obj.name + "_Parent");

            parent.transform.position = obj.transform.position;
            parent.transform.rotation = obj.transform.rotation;
            parent.transform.localScale = obj.transform.localScale;

            parent.transform.SetParent(obj.transform.parent);

            obj.transform.SetParent(parent.transform);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }
    }
}