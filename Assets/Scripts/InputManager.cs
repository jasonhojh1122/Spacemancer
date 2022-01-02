
using UnityEngine;

public class InputManager : MonoBehaviour {

    static InputManager _instance;
    public static InputManager Instance {
        get => _instance;
    }

    [SerializeField] Core.World world;
    [SerializeField] bool locked;
    [SerializeField] Skill.SkillController skillController;
    [SerializeField] Character.PlayerController playerController;
    [SerializeField] PlayerInteraction playerInteraction;

    public bool pause;

    private void Awake() {
        pause = false;
        skillController = FindObjectOfType<Skill.SkillController>();
        playerController = FindObjectOfType<Character.PlayerController>();
    }

    private void Update() {
        skillController.pause = pause;
        playerController.controlEnabled = !pause;
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
