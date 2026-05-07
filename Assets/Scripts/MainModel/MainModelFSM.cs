using System;

public class MainModelFSM
{
    public MainModelMode CurrentMode { get; private set; }

    public event Action<MainModelMode> OnModeChanged;

    public void SetMode(MainModelMode newMode)
    {
        if (CurrentMode == newMode) return;

        CurrentMode = newMode;
        OnModeChanged?.Invoke(newMode);
    }
}