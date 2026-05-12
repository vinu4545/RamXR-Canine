using UnityEngine;
using System.Collections.Generic;

public class ELDModeSwitchController : MonoBehaviour
{
    [Header("Event Channels")]
    public ELDModeSwitchedEventChannelSO modeChangeEvent;

    private ELDFSM fsm;

    [Header("Mode Configs")]
    public List<ELDModeConfigSO> modeConfigs;

    private Dictionary<ELDMode, ELDModeConfigSO> configMap;

    // [Header("Systems")]
    // public GameObject grabbableRoot;
    // public GameObject hoverSystem;
    // public GameObject labelSystem;
    // public GameObject deepVisionUI;

    void Awake()
    {
        fsm = new ELDFSM();
        fsm.OnModeChanged += ApplyMode;

        BuildConfigMap();
        // Ensure initial mode is applied
    }

    void Start()
    {
        fsm.Reset();
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
        configMap = new Dictionary<ELDMode, ELDModeConfigSO>();

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

    void OnModeRequested(ELDMode mode)
    {
        fsm.SetMode(mode);
    }

    void ApplyMode(ELDMode mode)
    {
        if (!configMap.ContainsKey(mode))
        {
            Debug.LogError($"No ModeConfig found for {mode}");
            return;
        }

        var config = configMap[mode];

        ApplyConfig(config);
    }

    void ApplyConfig(ELDModeConfigSO config)
    {
        // Don’t scatter logic—everything flows from config
        // LabelSystem.Instance.SetEnabled();
        GrabbableSystem.Instance.SetEnabled(config.enableGrabbable);
        MajorLabelSystem.Instance.SetEnabled(config.enableMajorLabels);

        // if (hoverSystem != null)
        //     hoverSystem.SetActive(config.enableHover);

        // if (labelSystem != null)
        //     labelSystem.SetActive(config.enableLabels);

        // if (deepVisionUI != null)
        //     deepVisionUI.SetActive(config.enableDeepVisionUI);
    }
}