using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ModelPart : MonoBehaviour
{
    [Header("Data")]
    public ModelPartDataSO partData;

    [Header("Event Channel")]
    public PartFocusEnteredEventChannelSO focusEventChannel;

    [Header("References")]
    public GameObject label;
    public XRBaseInteractable interactable;

    public string Id => partData != null ? partData.id : string.Empty;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (label != null)
            label.SetActive(false);
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnFocusEntered);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnFocusEntered);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (label != null)
            label.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (label != null)
            label.SetActive(false);
    }

    private void OnFocusEntered(SelectEnterEventArgs args)
    {
        if (partData != null && focusEventChannel != null)
        {
            focusEventChannel.RaiseEvent(partData);
        }
    }
}