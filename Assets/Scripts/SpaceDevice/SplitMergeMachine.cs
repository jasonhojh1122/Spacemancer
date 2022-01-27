using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using System.Linq;
using System.Collections.Generic;

using Core;

namespace SpaceDevice
{
    public class SplitMergeMachine : InputController
    {

        [SerializeField] bool withdrawerOn = false;
        [SerializeField] bool dimChangerOn = false;
        [SerializeField] List<UnityEngine.UI.Button> dimEnterButtons;
        [SerializeField] List<UnityEngine.UI.Button> dimChangeButtons;
        [SerializeField] UnityEngine.UI.Button splitMergeToggle;
        [SerializeField] UnityEngine.UI.Button withdrawerToggle;
        [SerializeField] UnityEngine.UI.Button withdrawerChangeButton;
        [SerializeField] List<Image> dimColorIndicators;

        [SerializeField] GameObject dimChanger;
        [SerializeField] GameObject withdrawer;

        InputAction toggleAction;
        UI.CanvasGroupFader fader;
        List<int> dimColorIds;

        public List<int> DimColorIds {
            get => dimColorIds;
        }

        private new void Awake()
        {
            base.Awake();

            fader = GetComponent<UI.CanvasGroupFader>();

            toggleAction = playerInput.actions["Toggle"];
            toggleAction.performed += Toggle;

            if (!withdrawerOn)
                withdrawer.SetActive(false);
            if (!dimChangerOn)
                dimChanger.SetActive(false);

            dimColorIds = new List<int>();
            for (int i = 0; i < dimEnterButtons.Count; i++)
            {
                dimColorIds.Add(i);
                UpdateDimColorIndicator(i);
            }
        }

        void Toggle(InputAction.CallbackContext context)
        {
            fader.Toggle();
            InputManager.Instance.ToggleGameplayInput(fader.IsOn);
        }

        public void SyncColorFromWorld()
        {
            for (int i = 0; i < dimColorIds.Count; i++)
            {
                if (World.Instance.Dimensions[i].Color == Dimension.Color.NONE)
                    dimColorIds[i] = -1;
                else
                    dimColorIds[i] = Dimension.ValidColorIndex[World.Instance.Dimensions[i].Color];
                UpdateDimColorIndicator(i);
            }
        }

        /// <summary>
        /// Rotates the dimension color to the next one if world is not splitted;
        /// </summary>
        /// <param name="i"> The index of the dimension to change. </param>
        public void RotateColor(int i)
        {
            if (World.Instance.Splitted) return;
            if (dimColorIds[i] == -1)
                dimColorIds[i] = 0;
            else
                dimColorIds[i] = (dimColorIds[i] + 1) % Dimension.ValidColor.Count;

            UpdateDimColorIndicator(i);
        }

        void UpdateDimColorIndicator(int i)
        {
            if (dimColorIds[i] == -1)
                dimColorIndicators[i].color = Dimension.MaterialColor[Dimension.Color.BLACK];
            else
                dimColorIndicators[i].color = Dimension.MaterialColor[Dimension.ValidColor[dimColorIds[i]]];
        }

        /// <summary>
        /// Enters the dimension when the world is splitted;
        /// </summary>
        /// <param name="i"> The index of the dimension to enter. </param>
        public void EnterDimension(int i)
        {
            if (!World.Instance.Splitted) return;
            World.Instance.ActiveDimId = i;
        }

        public void SplitMerge()
        {
            if (!World.Instance.Splitted)
            {
                if (dimColorIds[World.Instance.ActiveDimId]  == -1)
                    return;
                var c = Dimension.Color.NONE;
                for (int i = 0; i < dimColorIds.Count; i++)
                    if (dimColorIds[i] != -1)
                        c = Dimension.AddColor(c, Dimension.ValidColor[dimColorIds[i]]);
                if (c == Dimension.Color.BLACK)
                    return;
            }
            World.Instance.Toggle();
        }

    }
}

