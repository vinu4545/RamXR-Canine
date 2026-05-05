using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Main Model Mode Switched Event Channel")]
public class MainModelModeSwitchedEventChannelSO : ScriptableObject
{
    public UnityAction<MainModelMode> OnEventRaised;

    public void RaiseEvent(MainModelMode mode)
    {
        OnEventRaised?.Invoke(mode);
    }
}