using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    bool interactable = false;
    public bool alreadyInterate = false;
    Interaction interactableObject;
    Interaction child;
    public void Interact()
    {
        if (alreadyInterate)
        {
            alreadyInterate = false;
            interactableObject.Interact();
        }
        else if (interactable)
        {
            interactable = interactableObject.Interact();
            alreadyInterate = !alreadyInterate;
            
        }

    }
    private void OnTriggerEnter(Collider other)
    {

        if (alreadyInterate)
        {
            return;
        }
        if(other.gameObject.tag == "Boxes")
        {
            interactableObject = other.gameObject.GetComponent<Interaction>();
            interactable = interactableObject.interactable;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        interactable = false;
    }
}
