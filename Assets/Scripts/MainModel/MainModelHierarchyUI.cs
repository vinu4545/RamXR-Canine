// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class MainModelHierarchyUI : MonoBehaviour
// {
//     [Header("Mode Buttons")]
//     [SerializeField] private Button exploreButton;
//     [SerializeField] private Button separateViewButton;
//     [SerializeField] private Button deepVisionButton;

//     [Header("Scene Buttons")]
//     [SerializeField] private Button exitButton;
//     [SerializeField] private Button sceneButton1;
//     [SerializeField] private Button sceneButton2;
//     [SerializeField] private Button sceneButton3;
//     [SerializeField] private Button sceneButton4;

//     [Header("Event Channel")]
//     [SerializeField] private MainModelModeSwitchedEventChannelSO modeEvent;

//     [Header("Scene Names")]
//     [SerializeField] private string exitSceneName;
//     [SerializeField] private string scene1Name;
//     [SerializeField] private string scene2Name;
//     [SerializeField] private string scene3Name;
//     [SerializeField] private string scene4Name;

//     void Awake()
//     {
//         BindButtons();
//     }

//     void BindButtons()
//     {
//         Debug.Log("Binding buttons...");
//         // Mode Buttons
//         if (exploreButton != null)
//         {
//             Debug.Log("Binding Explore button");
//             exploreButton.onClick.AddListener(OnExploreClicked);
//         }
//         else
//         {
//             Debug.LogWarning("Explore button is not assigned.");
//         }
//         if (separateViewButton != null)
//             separateViewButton.onClick.AddListener(OnSeparateViewClicked);

//         if (deepVisionButton != null)
//             deepVisionButton.onClick.AddListener(OnDeepVisionClicked);

//         // Scene Buttons
//         if (exitButton != null)
//             exitButton.onClick.AddListener(OnExitClicked);

//         if (sceneButton1 != null)
//             sceneButton1.onClick.AddListener(OnScene1Clicked);

//         if (sceneButton2 != null)
//             sceneButton2.onClick.AddListener(OnScene2Clicked);

//         if (sceneButton3 != null)
//             sceneButton3.onClick.AddListener(OnScene3Clicked);

//         if (sceneButton4 != null)
//             sceneButton4.onClick.AddListener(OnScene4Clicked);
//     }

//     // 🔹 Mode Handlers

//     public void OnExploreClicked()
//     {
//         Debug.Log("Explore mode requested");
//         modeEvent?.RaiseEvent(MainModelMode.Explore);
//     }

//     public void OnSeparateViewClicked()
//     {
//         Debug.Log("Separate view mode requested");
//         modeEvent?.RaiseEvent(MainModelMode.SeparateView);
//     }

//     public void OnDeepVisionClicked()
//     {
//         Debug.Log("Deep vision mode requested");
//         modeEvent?.RaiseEvent(MainModelMode.DeepVision);
//     }

//     // 🔹 Scene Handlers

//     void OnExitClicked()
//     {
//         LoadScene(exitSceneName);
//     }

//     void OnScene1Clicked()
//     {
//         LoadScene(scene1Name);
//     }

//     void OnScene2Clicked()
//     {
//         LoadScene(scene2Name);
//     }

//     void OnScene3Clicked()
//     {
//         LoadScene(scene3Name);
//     }

//     void OnScene4Clicked()
//     {
//         LoadScene(scene4Name);
//     }

//     void LoadScene(string sceneName)
//     {
//         Debug.Log($"Loading scene: {sceneName}");
//         // if (string.IsNullOrEmpty(sceneName))
//         // {
//         //     Debug.LogWarning("Scene name is not assigned.");
//         //     return;
//         // }

//         // SceneManager.LoadScene(sceneName);
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainModelHierarchyUI : MonoBehaviour
{
    [Header("Mode Buttons")]
    [SerializeField] private Button exploreButton;
    [SerializeField] private Button separateViewButton;
    [SerializeField] private Button deepVisionButton;

    [Header("Scene Buttons")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button sceneButton1;
    [SerializeField] private Button sceneButton2;
    [SerializeField] private Button sceneButton3;
    [SerializeField] private Button sceneButton4;

    [Header("Event Channel")]
    [SerializeField] private MainModelModeSwitchedEventChannelSO modeEvent;

    [Header("Scene Names")]
    [SerializeField] private string exitSceneName;
    [SerializeField] private string scene1Name;
    [SerializeField] private string scene2Name;
    [SerializeField] private string scene3Name;
    [SerializeField] private string scene4Name;

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

        if (separateViewButton != null)
            separateViewButton.onClick.AddListener(OnSeparateViewClicked);

        if (deepVisionButton != null)
            deepVisionButton.onClick.AddListener(OnDeepVisionClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (sceneButton1 != null)
            sceneButton1.onClick.AddListener(OnScene1Clicked);

        if (sceneButton2 != null)
            sceneButton2.onClick.AddListener(OnScene2Clicked);

        if (sceneButton3 != null)
            sceneButton3.onClick.AddListener(OnScene3Clicked);

        if (sceneButton4 != null)
            sceneButton4.onClick.AddListener(OnScene4Clicked);
    }

    // 🔹 Mode Handlers

    public void OnExploreClicked()
    {
        modeEvent?.RaiseEvent(MainModelMode.Explore);
        ToggleDeepVisionUI(false);
    }

    public void OnSeparateViewClicked()
    {
        modeEvent?.RaiseEvent(MainModelMode.SeparateView);
        ToggleDeepVisionUI(false);
    }

    public void OnDeepVisionClicked()
    {
        modeEvent?.RaiseEvent(MainModelMode.DeepVision);
        ToggleDeepVisionUI(true);
    }

    void ToggleDeepVisionUI(bool state)
    {
        if (deepVisionUI != null)
            deepVisionUI.SetActive(state);
    }

    // 🔹 Scene Handlers

    void OnExitClicked() => LoadScene(exitSceneName);
    void OnScene1Clicked() => LoadScene(scene1Name);
    void OnScene2Clicked() => LoadScene(scene2Name);
    void OnScene3Clicked() => LoadScene(scene3Name);
    void OnScene4Clicked() => LoadScene(scene4Name);

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

        // SceneManager.LoadScene(sceneName);
    }
}