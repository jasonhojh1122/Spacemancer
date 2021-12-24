using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] UI.CanvasGroupFader hintUI;
    Vector3 hintUIOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Interaction.Interactable interactable;
    Core.SplittableObject interactableSo;
    Collider interactableCol;
    public float interactAngle = 50f;
    bool canInteract;

    void Update()
    {
        if (interactable != null)
        {
            if ( (interactableSo == null || interactableSo.IsInCorrectDim()) &&
                Vector3.Angle(interactable.gameObject.transform.position - transform.position, transform.forward) < interactAngle)
            {
                canInteract = true;
            }
            else
            {
                canInteract = false;
            }
            if (canInteract && !interactable.IsInteracting())
            {
                hintUI.FadeIn();
            }
            else
            {
                hintUI.FadeOut();
            }
        }
        else
        {
            hintUI.FadeOut();
            canInteract = false;
        }
    }

    public void Interact()
    {
        if (IsInteracting() || (interactable != null && canInteract))
        {
            interactable.Interact();
            canInteract = false;
        }
    }

    public bool IsInteracting()
    {
        if (interactable == null) return false;
        else return interactable.IsInteracting();
    }

    public void OnDimensionChange()
    {
        interactable = null;
    }

    public void SetInteractable(Interaction.Interactable newInteractable)
    {
        if (interactable != null && interactable.IsInteracting())
            return;
        interactable = newInteractable;
        if (interactable != null)
        {
            interactableCol = interactable.GetComponent<Collider>();
            interactableSo = interactable.GetComponent<Core.SplittableObject>();
        }
    }

    public void ClearInteractable(Interaction.Interactable oldInteractable)
    {
        if (interactable == null)
            return;
        else if (interactable.gameObject.GetInstanceID() == oldInteractable.gameObject.GetInstanceID())
            interactable = null;
    }
}
