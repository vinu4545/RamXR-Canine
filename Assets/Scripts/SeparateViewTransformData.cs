using UnityEngine;

[System.Serializable]
public class SeparateViewTransformData
{
    public string targetId;

    public Vector3 inactivePosition;
    public Vector3 inactiveRotation;
    public Vector3 inactiveScale;

    public Vector3 activePosition;
    public Vector3 activeRotation;
    public Vector3 activeScale;
}