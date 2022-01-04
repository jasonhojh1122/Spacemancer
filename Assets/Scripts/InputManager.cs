
using UnityEngine;

public class InputManager : MonoBehaviour {

    static InputManager _instance;
    public static InputManager Instance {
        get => _instance;
    }

    [SerializeField] Core.World world;
    [SerializeField] public bool pause;
    [SerializeField] PlayerInteraction playerInteraction;

    private void Awake() {
        _instance = this;
    }

    private void Update() {
        if (pause) return;
        if (!playerInteraction.IsInteracting()) {
            if (Input.GetButtonUp("WorldToggle"))
            {
                world.Toggle();
            }
            else if (Input.GetButtonUp("WorldShiftLeft"))
            {
                world.RotateDimensions(-1);
            }
            else if (Input.GetButtonUp("WorldShiftRight"))
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
