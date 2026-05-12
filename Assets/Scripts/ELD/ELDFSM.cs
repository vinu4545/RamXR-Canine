using System;

public class ELDFSM
{
    public ELDMode CurrentMode { get; private set; }

    public event Action<ELDMode> OnModeChanged;

    public void SetMode(ELDMode newMode)
    {
        if (CurrentMode == newMode) return;

        CurrentMode = newMode;
        OnModeChanged?.Invoke(newMode);
    }

    public void Reset()
    {
        CurrentMode = ELDMode.Explore;
        OnModeChanged?.Invoke(CurrentMode);
    }
}