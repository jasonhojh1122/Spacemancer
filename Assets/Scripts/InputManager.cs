
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
            if (Input.GetButtonUp("Toggle"))
            {
                world.Toggle();
            }
            else if (Input.GetButtonUp("L-Rotate"))
            {
                world.RotateDimensions(-1);
            }
            else if (Input.GetButtonUp("R-Rotate"))
            {
                world.RotateDimensions(1);
            }
        }
        if (Input.GetButtonDown("Interact"))
        {
            playerInteraction.Interact();
        }
    }

}
