
using UnityEngine;

public class InputManager : MonoBehaviour {

    static InputManager _instance;
    public static InputManager Instance {
        get => _instance;
    }

    [SerializeField] Core.World world;
    [SerializeField] bool locked;
    [SerializeField] PlayerInteraction playerInteraction;

    public bool pause;

    private void Awake() {
        pause = false;
        _instance = this;
    }

    private void Update() {
        if (pause) return;
        if(Input.GetButtonDown("Hint")){
            FindObjectOfType<HintPageController>().EnableHintPage();
        }
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
