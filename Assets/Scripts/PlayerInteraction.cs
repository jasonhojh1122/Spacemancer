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
            if(Vector3.Angle(interactable.gameObject.transform.position - gameObject.transform.position,gameObject.transform.forward ) < interactAngle)
            {
                interactable.Interact();
            }
            
        }
    }

    public bool IsInteracting() {
        if (interactable == null) return false;
        else return interactable.IsInteracting();
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
        if (interactable != null  && other.transform.parent.gameObject == interactable.gameObject)
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
