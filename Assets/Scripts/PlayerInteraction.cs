using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Interactable interactableObject;
    public void Interact()
    {
        if (interactableObject != null)
        {
            interactableObject.Interact();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Interactable _interactableObject = other.GetComponent<Interactable>();
        if(_interactableObject != null && interactableObject != null)
        {
            interactableObject = _interactableObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactableObject = null;
    }
}
