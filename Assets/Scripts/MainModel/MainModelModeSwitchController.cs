using UnityEngine;
using System.Collections.Generic;

public class MainModelModeSwitchController : MonoBehaviour
{
    [Header("Event Channels")]
    public MainModelModeSwitchedEventChannelSO modeChangeEvent;

    private MainModelFSM fsm;

    [Header("Mode Configs")]
    public List<MainModelModeConfigSO> modeConfigs;

    private Dictionary<MainModelMode, MainModelModeConfigSO> configMap;

    // [Header("Systems")]
    // public GameObject grabbableRoot;
    // public GameObject hoverSystem;
    // public GameObject labelSystem;
    // public GameObject deepVisionUI;

    void Awake()
    {
        fsm = new MainModelFSM();
        fsm.OnModeChanged += ApplyMode;

        BuildConfigMap();
    }

    void OnEnable()
    {
        modeChangeEvent.OnEventRaised += OnModeRequested;
    }

    void OnDisable()
    {
        modeChangeEvent.OnEventRaised -= OnModeRequested;
    }

    void BuildConfigMap()
    {
        configMap = new Dictionary<MainModelMode, MainModelModeConfigSO>();

        foreach (var config in modeConfigs)
        {
            if (config == null) continue;

            if (!configMap.ContainsKey(config.mode))
            {
                configMap.Add(config.mode, config);
            }
            else
            {
                Debug.LogWarning($"Duplicate ModeConfig for {config.mode}");
            }
        }
    }

    void OnModeRequested(MainModelMode mode)
    {
        fsm.SetMode(mode);
    }

    void ApplyMode(MainModelMode mode)
    {
        if (!configMap.ContainsKey(mode))
        {
            Debug.LogError($"No ModeConfig found for {mode}");
            return;
        }

        var config = configMap[mode];

        ApplyConfig(config);
    }

    void ApplyConfig(MainModelModeConfigSO config)
    {
        // Don’t scatter logic—everything flows from config
        GrabbableSystem.Instance.SetEnabled(config.enableGrabbable);
        SeparateViewSystem.Instance.SetEnabled(config.enableSeparateView);

        // if (hoverSystem != null)
        //     hoverSystem.SetActive(config.enableHover);

        // if (labelSystem != null)
        //     labelSystem.SetActive(config.enableLabels);

        // if (deepVisionUI != null)
        //     deepVisionUI.SetActive(config.enableDeepVisionUI);
    }
}