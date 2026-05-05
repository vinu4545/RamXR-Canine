using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ModelPartSocketInteractor : XRSocketInteractor
{
    [Header("Validation")]
    public string allowedId;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (!base.CanHover(interactable))
            return false;

        return IsValid(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        if (!base.CanSelect(interactable))
            return false;

        return IsValid(interactable);
    }

    private bool IsValid(IXRInteractable interactable)
    {
        var component = interactable.transform.GetComponent<ModelPart>();

        if (component == null || component.partData == null)
            return false;

        return component.partData.id == allowedId;
    }
}