
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [RequireComponent(typeof(UI.CanvasGroupFader))]
    public class PauseMenuController : InputController
    {
        UI.CanvasGroupFader fader;
        InputAction toggleAction;

        bool isOpened;
        new void Awake()
        {
            base.Awake();
            isOpened = false;
            fader = GetComponent<UI.CanvasGroupFader>();
            toggleAction = playerInput.actions["Toggle"];
            toggleAction.performed += Toggle;
        }

        public void Toggle(InputAction.CallbackContext callbackContext)
        {
            if (isOpened)
            {
                fader.FadeOut();
                isOpened = false;
            }
            else
            {
                fader.FadeIn();
                isOpened = true;
            }
        }


    }
}
