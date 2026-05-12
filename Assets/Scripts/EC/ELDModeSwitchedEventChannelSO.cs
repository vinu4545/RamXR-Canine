using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/ELD Mode Switched Event Channel")]
public class ELDModeSwitchedEventChannelSO : ScriptableObject
{
    public UnityAction<ELDMode> OnEventRaised;

    public void RaiseEvent(ELDMode mode)
    {
        OnEventRaised?.Invoke(mode);
    }
}