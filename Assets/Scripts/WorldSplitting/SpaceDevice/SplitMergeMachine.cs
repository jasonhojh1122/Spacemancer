using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using System.Collections.Generic;

namespace Core.SpaceDevice
{
    public class SplitMergeMachine : InputController
    {

        [SerializeField] bool colorWithdrawerOn = false;
        [SerializeField] bool dimensionColorSelectorOn = false;
        [SerializeField] List<UnityEngine.UI.Button> dimensionButtons;
        [SerializeField] List<Image> dimensionColors;
        [SerializeField] UnityEngine.UI.Button splitMergeToggle;

        [SerializeField] GameObject dimensionColorSelector;
        [SerializeField] GameObject colorWithdrawer;

        InputAction toggleAction;
        UI.CanvasGroupFader fader;

        private new void Awake()
        {
            base.Awake();

            fader = GetComponent<UI.CanvasGroupFader>();

            toggleAction = playerInput.actions["Toggle"];
            toggleAction.performed += Toggle;

            if (!colorWithdrawerOn)
                colorWithdrawer.SetActive(false);
            if (!dimensionColorSelectorOn)
                dimensionColorSelector.SetActive(false);
        }

        void Toggle(InputAction.CallbackContext context)
        {
            fader.Toggle();
            InputManager.Instance.ToggleGameplayInput(fader.IsOn);
        }

    }
}

