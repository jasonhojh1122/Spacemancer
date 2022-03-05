using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

using System.Collections.Generic;

using Core;
using Input;

namespace SpaceDevice
{
    public class SplitMergeMachine : InputController
    {
        [SerializeField] List<Image> dimColorIndicators;
        [SerializeField] VisualEffect playerTransitionVFX;
        [SerializeField] CameraController cameraController;
        [SerializeField] float enterDimensionDuration;

        [Header("Hinting UI")]
        [SerializeField] List<Image> hintImages;
        [SerializeField] Animator hintAnimator;

        InputAction toggleAction;
        UI.CanvasGroupFader fader;
        List<int> dimColorIds;

        public List<int> DimColorIds
        {
            get => dimColorIds;
        }

        public List<Image> DimColorIndicators
        {
            get => dimColorIndicators;
        }

        private new void Awake()
        {
            base.Awake();

            fader = GetComponent<UI.CanvasGroupFader>();

            toggleAction = playerInput.actions["Toggle"];
            toggleAction.performed += Toggle;

            dimColorIds = new List<int>();
            for (int i = 0; i < Dimension.BaseColor.Count; i++)
            {
                dimColorIds.Add(i);
                UpdateDimColorIndicator();
            }
            Core.World.Instance.OnTransitionStart.AddListener(PlayHintAnim);
            Core.World.Instance.OnTransitionEnd.AddListener(UpdateHintColor);
            Core.World.Instance.OnActiveDimChange.AddListener(UpdateHintColor);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (IsPaused()) return;
            fader.Toggle();
            InputManager.Instance.ToggleGameplayInput(fader.IsOn);
        }

        public void Toggle()
        {
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
        /// Updates the dimension indicators images' color.
        /// </summary>
        public void UpdateDimColorIndicator()
        {
            for (int i = 0; i < dimColorIds.Count; i++)
            {
                if (dimColorIds[i] < 0)
                    dimColorIndicators[i].color = Dimension.MaterialColor[Dimension.Color.BLACK];
                else
                    dimColorIndicators[i].color = Dimension.MaterialColor[Dimension.SplittedColor[dimColorIds[i]]];
            }
        }

        /// <summary>
        /// Gets the Dimension.Color based on the dimension index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Dimension.Color GetColorByID(int i)
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
                || i == World.Instance.ActiveDimId || !EnergyBar.Instance.IsSufficient()) return;
            EnergyBar.Instance.CostSingleAction();
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

        private void PlayHintAnim()
        {
            hintAnimator.SetBool("IsSplitted", Core.World.Instance.Splitted);
        }

        private void UpdateHintColor()
        {
            foreach (var img in hintImages)
            {
                img.color = Core.Dimension.MaterialColor[Core.World.Instance.ActiveDimension.color];
            }
        }

    }
}

