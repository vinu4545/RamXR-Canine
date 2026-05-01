using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DeepVisionUIController : MonoBehaviour
{
    public DeepVisionTutorialSO tutorialData;

    public TextMeshProUGUI tutorialText;

    public Transform buttonContainer;
    public GameObject buttonPrefab;
    public IntEventChannelSO stepChangedEvent;


    private List<TutorialStepButton> buttons = new List<TutorialStepButton>();
    private int currentIndex = 0;

    void Start()
    {
        GenerateButtons();
        ShowStep(0);
    }

    void GenerateButtons()
    {
        int count = tutorialData.steps.Length;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(buttonPrefab, buttonContainer);
            var btn = obj.GetComponent<TutorialStepButton>();

            int index = i; // closure safety
            btn.Setup(index, OnStepClicked);

            buttons.Add(btn);
        }
    }

    void OnStepClicked(int index)
    {
        ShowStep(index);
    }

    public void Next()
    {
        if (currentIndex < tutorialData.steps.Length - 1)
            ShowStep(currentIndex + 1);
    }

    public void Previous()
    {
        if (currentIndex > 0)
            ShowStep(currentIndex - 1);
    }

    void ShowStep(int index)
    {
        currentIndex = index;

        // Update text
        tutorialText.text = tutorialData.steps[index];

        // Update buttons
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(i <= index);
        }
        stepChangedEvent.RaiseEvent(index);
    }
}