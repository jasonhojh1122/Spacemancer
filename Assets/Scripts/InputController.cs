
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    protected PlayerInput playerInput;

    public bool pause;

    protected void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    protected bool IsPaused()
    {
        return pause || InputManager.Instance.pause;
    }

}