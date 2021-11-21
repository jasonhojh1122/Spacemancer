using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Interaction.Interactable interactable;
    public float interactAngle = 50f;
    bool exit = false;
    public void Interact()
    {
        if (interactable != null)
        {
            if(Vector3.Angle(interactable.gameObject.transform.position - transform.position, transform.forward ) < interactAngle)
            {
                interactable.Interact();
            }

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
        exit = false;
    }

    private void OnTriggerEnter(Collider other)
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
    }
}
