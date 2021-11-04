using UnityEngine;

namespace Interaction {
public class Zone : MonoBehaviour {

    [SerializeField] Interactable interactable;
    Collider col;

    public Interactable InteractableObect {
        get => interactable;
    }

    private void Awake() {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        interactable.OnZoneEnter(other);
    }
    private void OnTriggerStay(Collider other) {
        interactable.OnZoneStay(other);
    }
    private void OnTriggerExit(Collider other) {
        interactable.OnZoneExit(other);
    }

}

}