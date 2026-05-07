using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Deep Vision/Separate View Data")]
public class SeparateViewDataSO : ScriptableObject
{
    public List<SeparateViewTransformData> elements;
    public float transitionDuration = 1f;
}