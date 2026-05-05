using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InformationPanelUI : MonoBehaviour
{
    [Header("Event Channel")]
    public PartFocusEnteredEventChannelSO focusEventChannel;

    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button previousButton;
    public Button nextButton;

    private ModelPartDataSO currentData;
    private int currentIndex;

    private void OnEnable()
    {
        focusEventChannel.OnEventRaised += OnPartSelected;
        previousButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);
    }

    private void OnDisable()
    {
        focusEventChannel.OnEventRaised -= OnPartSelected;
        previousButton.onClick.RemoveListener(ShowPrevious);
        nextButton.onClick.RemoveListener(ShowNext);
    }

    private void OnPartSelected(ModelPartDataSO data)
    {
        currentData = data;
        currentIndex = 0;

        UpdateUI();
    }

    private void ShowPrevious()
    {
        if (currentData == null) return;

        currentIndex--;
        UpdateUI();
    }

    private void ShowNext()
    {
        if (currentData == null) return;

        currentIndex++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentData == null || currentData.descriptionChunks.Length == 0)
            return;

        titleText.text = currentData.partName;
        descriptionText.text = currentData.descriptionChunks[currentIndex];

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < currentData.descriptionChunks.Length - 1;
    }
}