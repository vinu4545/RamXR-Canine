using UnityEngine;

[CreateAssetMenu(menuName = "Modes/ELD Mode Config")]
public class ELDModeConfigSO : ScriptableObject
{
    public ELDMode mode;

    [Header("Feature Toggles")]
    public bool enableGrabbable;
    public bool enableHover;
    public bool enableLabels;
    public bool enableDeepVisionUI;
    public bool enableMajorLabels;
}