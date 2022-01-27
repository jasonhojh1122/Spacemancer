
using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    static InputManager _instance;
    public static InputManager Instance {
        get => _instance;
    }

    [SerializeField] List<InputController> uiInputs;
    [SerializeField] List<InputController> gameplayInputs;

    [SerializeField] Core.World world;
    [SerializeField] public bool pause;
    [SerializeField] PlayerInteraction playerInteraction;

    public void ToggleGameplayInput(bool isPaused)
    {
        foreach (var input in gameplayInputs)
        {
            input.pause = isPaused;
        }
    }

    public void ToggleUIInput(bool isPaused)
    {
        foreach (var input in uiInputs)
        {
            input.pause = isPaused;
        }
    }

    private void Awake() {
        _instance = this;
    }

    private void Update() {

    }

}
