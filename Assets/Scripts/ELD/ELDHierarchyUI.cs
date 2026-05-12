using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ELDHierarchyUI : MonoBehaviour
{
    [Header("Mode Buttons")]
    [SerializeField] private Button exploreButton;
    [SerializeField] private Button labelsButton;
    [SerializeField] private Button deepVisionButton;

    [Header("Scene Buttons")]
    [SerializeField] private Button exitButton;

    [Header("Event Channel")]
    [SerializeField] private ELDModeSwitchedEventChannelSO modeEvent;

    [Header("Scene Names")]
    [SerializeField] private string exitSceneName;

    [Header("UI References")]
    [SerializeField] private GameObject deepVisionUI;

    void Awake()
    {
        BindButtons();

        // Ensure correct initial state
        if (deepVisionUI != null)
            deepVisionUI.SetActive(false);
    }

    void BindButtons()
    {
        if (exploreButton != null)
            exploreButton.onClick.AddListener(OnExploreClicked);

        if (labelsButton != null)
            labelsButton.onClick.AddListener(OnLabelsClicked);

        if (deepVisionButton != null)
            deepVisionButton.onClick.AddListener(OnDeepVisionClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    // 🔹 Mode Handlers

    public void OnExploreClicked()
    {
        modeEvent?.RaiseEvent(ELDMode.Explore);
        ToggleDeepVisionUI(false);
    }

    public void OnLabelsClicked()
    {
        modeEvent?.RaiseEvent(ELDMode.Labels);
        ToggleDeepVisionUI(false);
    }

    public void OnDeepVisionClicked()
    {
        modeEvent?.RaiseEvent(ELDMode.DeepVision);
        ToggleDeepVisionUI(true);
    }

    void ToggleDeepVisionUI(bool state)
    {
        if (deepVisionUI != null)
            deepVisionUI.SetActive(state);
    }

    // 🔹 Scene Handlers

    void OnExitClicked() => LoadScene(exitSceneName);

    void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");

        // You commented this out — that’s fine for testing,
        // but don’t forget to re-enable before shipping.

        // if (string.IsNullOrEmpty(sceneName))
        // {
        //     Debug.LogWarning("Scene name is not assigned.");
        //     return;
        // }

        SceneManager.LoadScene(sceneName);
    }
}