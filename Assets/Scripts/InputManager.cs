
using UnityEngine;

public class InputManager : MonoBehaviour {

    // moving camera
    // spliting or merging
    // moving character

    [SerializeField] World world;

    [SerializeField] bool locked;


    private void Update() {
        if (locked) return;

        if (Input.GetKeyUp(KeyCode.Space)) {
            world.Toggle();
        }
        else if (Input.GetKeyUp(KeyCode.Q)) {
            world.RotateDimensions(-1);
        }
        else if (Input.GetKeyUp(KeyCode.E)) {
            world.RotateDimensions(1);
        }
    }

    public void ToggleLock() {
        locked = !locked;
    }


}