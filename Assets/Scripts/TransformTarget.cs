using UnityEngine;

[System.Serializable]
public class TransformTarget
{
    public string targetId;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
}