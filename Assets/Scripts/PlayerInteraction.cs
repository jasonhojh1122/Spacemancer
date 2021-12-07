using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] GameObject hintUI;
    Vector3 hintUIOffset = new Vector3(0.0f, 1.0f, 0.0f);
    Interaction.Interactable interactable;
    Core.SplittableObject interactableSo;
    Collider interactableCol;
    public float interactAngle = 50f;
    bool canInteract;

    private void Start() {
        HideHintUI();
    }

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
                ShowHintUI();
            }
            else
            {
                HideHintUI();
            }
        }
        else
        {
            HideHintUI();
            canInteract = false;
        }
    }

    public void Interact()
    {
        if (interactable != null && canInteract)
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

    void ShowHintUI()
    {
        hintUI.SetActive(true);
        Vector3 pos = interactableCol.bounds.center;
        pos.y += interactableCol.bounds.extents.y;
        pos += hintUIOffset;
        hintUI.transform.position = pos;
    }

    void HideHintUI()
    {
        hintUI.SetActive(false);
    }


    /* private void OnTriggerEnter(Collider other)
    {
        if (exit && interactable.IsInteracting())
        {
            interactable = null;
            exit = false;
        }
        if (interactable != null)
            return;
        Interaction.Zone zone = other.GetComponent<Interaction.Zone>();
        if(zone != null)
        {
            interactable = zone.InteractableObect;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<Interaction.Zone>();
        if (interactable != null && zone != null && zone.InteractableObect.gameObject.GetInstanceID() == interactable.gameObject.GetInstanceID())
        {
            if (interactable.IsInteracting())
            {
                exit = true;
            }
            else
            {
                interactable = null;
            }

        }
    } */
}
