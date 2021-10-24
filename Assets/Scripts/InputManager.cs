
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
    }

    public void ToggleLock() {
        locked = !locked;
    }


}