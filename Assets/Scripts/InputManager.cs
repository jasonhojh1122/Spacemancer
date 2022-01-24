
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

    }

}
