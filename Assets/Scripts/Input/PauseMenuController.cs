
using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections.Generic;

namespace Input
{
    [RequireComponent(typeof(UI.CanvasGroupFader))]
    public class PauseMenuController : InputController
    {
        [SerializeField] List<UI.CanvasGroupFader> popupWindows;
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
                foreach (var popup in popupWindows)
                {
                    popup.FadeOut();
                }
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
