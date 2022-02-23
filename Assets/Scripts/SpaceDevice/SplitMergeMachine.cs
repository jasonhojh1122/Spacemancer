using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

using System.Linq;
using System.Collections.Generic;

using Core;

namespace SpaceDevice
{
    public class SplitMergeMachine : InputController
    {
        [Header("Sub-device Settings")]
        [SerializeField] bool withdrawerOn = false;
        [SerializeField] bool dimChangerOn = false;
        [SerializeField] List<Image> dimColorIndicators;
        [SerializeField] GameObject dimChanger;
        [SerializeField] GameObject withdrawer;

        [Header("Energy Cost Per Action")]
        [SerializeField] float energyCost = 0.1f;

        [Header("Enter Dimension Settings")]
        [SerializeField] VisualEffect playerTransitionVFX;
        [SerializeField] CameraController cameraController;
        [SerializeField] float enterDimensionDuration;

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
            for (int i = 0; i < Dimension.BaseColor.Count; i++)
            {
                dimColorIds.Add(i);
                UpdateDimColorIndicator();
            }
        }

        void Toggle(InputAction.CallbackContext context)
        {
            if (IsPaused()) return;
            fader.Toggle();
            InputManager.Instance.ToggleGameplayInput(fader.IsOn);
        }

        public void SyncColorFromWorld()
        {
            for (int i = 0; i < dimColorIds.Count; i++)
            {
                if (World.Instance.Dimensions[i].color == Dimension.Color.NONE)
                    dimColorIds[i] = -1;
                else
                    dimColorIds[i] = Dimension.ValidColorIndex[World.Instance.Dimensions[i].color];
            }
            UpdateDimColorIndicator();
        }

        /// <summary>
        /// Rotates the dimension color to the next one if world is not splitted;
        /// </summary>
        /// <param name="i"> The index of the dimension to change. </param>
        public void RotateColor(int i)
        {
            if (World.Instance.Splitted) return;
            RotateColorId(i);
            if (i == World.Instance.ActiveDimId)
            {
                var missingColor = Dimension.MissingColor(GetColorByID(i));
                for (int j = 0; j < dimColorIds.Count; j++)
                {
                    var id = (i+j+1) % dimColorIds.Count;
                    if (id == i)
                        continue;
                    else if (j >= missingColor.Count && id != i)
                        dimColorIds[id] = -1;
                    else
                        dimColorIds[id] = Dimension.ValidColorIndex[missingColor[j]];
                }
            }
            else
            {
                var missingColor = Dimension.MissingColor(GetColorByID(i));
                dimColorIds[World.Instance.ActiveDimId] = Dimension.ValidColorIndex[missingColor[0]];
                for (int j = 1; j < dimColorIds.Count; j++)
                {
                    var id = (i+j) % dimColorIds.Count;
                    if (id == World.Instance.ActiveDimId)
                        continue;
                    else if (j >= missingColor.Count)
                        dimColorIds[id] = -1;
                    else
                        dimColorIds[id] = Dimension.ValidColorIndex[missingColor[j]];
                }
            }
            UpdateDimColorIndicator();
        }

        void RotateColorId(int i)
        {
            if (dimColorIds[i] < 0)
                dimColorIds[i] = 0;
            else
                dimColorIds[i] = (dimColorIds[i] + 1) % Dimension.SplittedColor.Count;
        }

        void UpdateDimColorIndicator()
        {
            for (int i = 0; i < dimColorIds.Count; i++)
            {
                if (dimColorIds[i] < 0)
                    dimColorIndicators[i].color = Dimension.MaterialColor[Dimension.Color.BLACK];
                else
                    dimColorIndicators[i].color = Dimension.MaterialColor[Dimension.SplittedColor[dimColorIds[i]]];
            }
        }

        Dimension.Color GetColorByID(int i)
        {
            return Dimension.SplittedColor[dimColorIds[i]];
        }

        /// <summary>
        /// Enters the dimension when the world is splitted;
        /// </summary>
        /// <param name="i"> The index of the dimension to enter. </param>
        public void EnterDimension(int i)
        {
            if (!World.Instance.Splitted || dimColorIds[i] < 0
                || i == World.Instance.ActiveDimId) return;
            StartCoroutine(EnterAnim(i));
        }

        System.Collections.IEnumerator EnterAnim(int i)
        {
            fader.FadeOut();
            InputManager.Instance.pause = true;
            playerTransitionVFX.Stop();
            playerTransitionVFX.Play();
            cameraController.FollowPlayer();
            World.Instance.ActiveDimId = i;
            yield return new WaitForSeconds(enterDimensionDuration);
            cameraController.UnFollowPlayer();
            playerTransitionVFX.Stop();
            playerTransitionVFX.Play();
            InputManager.Instance.pause = false;
            InputManager.Instance.ToggleGameplayInput(fader.IsOn);
        }

        public void SplitMerge()
        {
            World.Instance.Toggle();
            fader.FadeOut();
            InputManager.Instance.ToggleGameplayInput(fader.IsOn);
        }

    }
}

