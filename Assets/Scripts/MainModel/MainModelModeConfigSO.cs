using UnityEngine;

[CreateAssetMenu(menuName = "Modes/Mode Config")]
public class MainModelModeConfigSO : ScriptableObject
{
    public MainModelMode mode;

    [Header("Feature Toggles")]
    public bool enableGrabbable;
    public bool enableHover;
    public bool enableLabels;
    public bool enableDeepVisionUI;
    public bool enableSeparateView;
}