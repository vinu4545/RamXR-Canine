using UnityEngine;
using System;

[CreateAssetMenu(menuName = "DeepVision/Event Channel/Int")]
public class IntEventChannelSO : ScriptableObject
{
    public event Action<int> OnEventRaised;

    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}