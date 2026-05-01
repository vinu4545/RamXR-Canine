using UnityEngine;
using System.Collections.Generic;

public class DeepVisionSceneBinder : MonoBehaviour
{
    [System.Serializable]
    public class Binding
    {
        public string id;
        public Transform target;
    }

    public Binding[] bindings;

    private Dictionary<string, Transform> lookup;

    void Awake()
    {
        lookup = new Dictionary<string, Transform>();

        foreach (var b in bindings)
        {
            if (!lookup.ContainsKey(b.id))
                lookup.Add(b.id, b.target);
        }
    }

    public Transform Get(string id)
    {
        return lookup.TryGetValue(id, out var t) ? t : null;
    }
}