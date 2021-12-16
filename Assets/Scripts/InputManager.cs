
using UnityEngine;

public class InputManager : MonoBehaviour {

    // moving camera
    // spliting or merging
    // moving character

    [SerializeField] Core.World world;

    [SerializeField] bool locked;
    [SerializeField] PlayerInteraction playerInteraction;
    private void Update() {
        if (!playerInteraction.IsInteracting()) {
            if (Input.GetButtonUp("WorldToggle"))
            {
                world.Toggle();
            }
            else if (Input.GetButtonUp("WorldShiftLeft"))
            {
                world.RotateDimensions(1);
            }
            else if (Input.GetButtonUp("WorldShiftRight"))
            {
                world.RotateDimensions(-1);
            }
        }
        if (Input.GetButtonDown("Interact"))
        {
            playerInteraction.Interact();
        }
    }

}
