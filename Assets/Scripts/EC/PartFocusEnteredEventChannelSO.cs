using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PartFocusEvent", menuName = "VR/Event Channels/Part Focus Entered")]
public class PartFocusEnteredEventChannelSO : ScriptableObject
{
    public Action<ModelPartDataSO> OnEventRaised;

    public void RaiseEvent(ModelPartDataSO data)
    {
        OnEventRaised?.Invoke(data);
    }
}