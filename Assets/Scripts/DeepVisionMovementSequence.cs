using UnityEngine;

[CreateAssetMenu(menuName = "DeepVision/Movement Sequence")]
public class DeepVisionMovementSequence : ScriptableObject
{
    public StepData[] steps;

    [System.Serializable]
    public class StepData
    {
        public TransformTarget[] targets;
        public float duration = 0.5f;
    }
}