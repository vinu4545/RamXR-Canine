using UnityEngine;
using System.Collections.Generic;

public class SeparateViewSceneBinder : MonoBehaviour
{
    [System.Serializable]
    public class Binding
    {
        public string targetId;
        public Transform targetTransform;
    }

    public List<Binding> bindings;

    private Dictionary<string, Transform> bindingDict;

    void Awake()
    {
        bindingDict = new Dictionary<string, Transform>();

        foreach (var b in bindings)
        {
            if (!bindingDict.ContainsKey(b.targetId))
            {
                bindingDict.Add(b.targetId, b.targetTransform);
            }
        }
    }

    public Transform GetTransform(string id)
    {
        return bindingDict.ContainsKey(id) ? bindingDict[id] : null;
    }
}