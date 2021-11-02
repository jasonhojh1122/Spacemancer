
using UnityEngine;

public class InputManager : MonoBehaviour {

    // moving camera
    // spliting or merging
    // moving character

    [SerializeField] World world;

    [SerializeField] bool locked;


    private void Update() {
        if (locked) return;

        if (Input.GetButtonUp("Toggle")) {
            world.Toggle();
        }
        else if (Input.GetButtonUp("L-Rotate")) {
            world.RotateDimensions(-1);
        }
        else if (Input.GetButtonUp("R-Rotate")) {
            world.RotateDimensions(1);
        }
    }

    public void ToggleLock() {
        locked = !locked;
    }


}