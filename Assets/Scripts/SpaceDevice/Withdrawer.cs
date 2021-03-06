
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using System.Collections.Generic;

using Core;
using Input;
using Gameplay.Interactable;

namespace SpaceDevice
{
    public class Withdrawer : InputController
    {
        enum WithdrawerState
        {
            OFF, TO_WITHDRAW, TO_INSERT, WAIT
        }
        [SerializeField] Laser laser;
        [SerializeField] RectTransform colorSelector;
        [SerializeField] UnityEngine.UI.Image withdrawContainer;
        [SerializeField] Animator hintAnimator;
        [SerializeField] UnityEngine.UI.Image hintCenter;
        [SerializeField] UnityEngine.UI.Image hintSide;
        [SerializeField] UnityEngine.Events.UnityEvent OnInsert;
        [SerializeField] UnityEngine.Events.UnityEvent OnWithdraw;

        static Withdrawer _instance;

        /// <summary>
        /// Current color for withdrawing.
        /// </summary>
        Dimension.Color withdrawColor;

        /// <summary>
        /// Withdrew color held by the skill.
        /// </summary>
        Dimension.Color holdColor;
        WithdrawerState curState;
        int withdrawColorIdx = 0;

        UnityEngine.Color32 transparentColor = new Color32(0, 0, 0, 0);

        InputAction toggle;
        InputAction perform;

        public static Withdrawer Instance
        {
            get => _instance;
        }

        public bool IsOn {
            get => laser.IsOn;
        }

        [HideInInspector] public UnityEvent OnToggle = new UnityEvent();

        new void Awake()
        {
            base.Awake();
            if (_instance != null)
                Debug.LogError("Multiple instances of Withdrawer created.");
            _instance = this;
            toggle = playerInput.actions["WithdrawerToggle"];
            perform = playerInput.actions["WithdrawerPerform"];
            curState = WithdrawerState.OFF;
            withdrawColor = Dimension.BaseColor[0];
            holdColor = Dimension.Color.NONE;
        }

        private void Update()
        {
            if (IsPaused()) return;
            if (toggle.triggered)
                Toggle();
            if (perform.triggered)
                Perform();
        }

        public void Toggle()
        {
            if (InteractionManager.Instance != null && InteractionManager.Instance.IsInteracting)
                return;
            if (curState == WithdrawerState.WAIT) return;
            if (curState == WithdrawerState.OFF)
            {
                Splittable.Character.Player.Instance.TakeOutSpaceDevice();
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        void TurnOn()
        {
            InteractionManager.Instance.pause = true;
            laser.IsOn = true;
            if (holdColor != Dimension.Color.NONE)
                curState = WithdrawerState.TO_INSERT;
            else
            {
                curState = WithdrawerState.TO_WITHDRAW;
                hintSide.color = Dimension.MaterialColor[withdrawColor];
            }
            laser.Color = withdrawColor;
            OnToggle.Invoke();
        }

        void TurnOff()
        {
            InteractionManager.Instance.pause = false;
            Splittable.Character.Player.Instance.PutAwaySpaceDevice();
            laser.IsOn = false;
            curState = WithdrawerState.OFF;
            OnToggle.Invoke();
            if (holdColor == Dimension.Color.NONE)
                hintSide.color = Dimension.MaterialColor[Dimension.Color.WHITE];
        }

        public void Perform()
        {
            if (curState == WithdrawerState.OFF)
                return;
            else if (curState == WithdrawerState.TO_WITHDRAW) // Try to withdraw the color.
            {
                if (laser.HittedObject != null &&
                    ((laser.HittedObject.Color & withdrawColor) != Dimension.Color.NONE) &&
                    EnergyBar.Instance.IsSufficient() )
                {
                    hintAnimator.SetTrigger("Withdraw");
                    hintCenter.color = Dimension.MaterialColor[withdrawColor];
                    EnergyBar.Instance.CostSingleAction();
                    Withdraw();
                }
                else
                    TurnOff();
            }
            else if (curState == WithdrawerState.TO_INSERT)
            {
                if (laser.HittedObject != null &&
                    ((laser.HittedObject.Color & holdColor) == Dimension.Color.NONE) &&
                    EnergyBar.Instance.IsSufficient() )
                {
                    hintAnimator.SetTrigger("Insert");
                    EnergyBar.Instance.CostSingleAction();
                    Insert();
                }
                else
                    TurnOff();
            }
        }

        /// <summary>
        /// Withdraws the <c>withdrawColor</c> from the <c>SplittableObject</c>
        /// hitted by the laser.
        /// </summary>
        private void Withdraw()
        {
            laser.HittedObject.SecondColor =  laser.HittedObject.Color;
            laser.HittedObject.Color = Dimension.SubColor(laser.HittedObject.Color, withdrawColor);
            laser.HittedObject.ContactPoint = laser.ContactPoint;
            laser.HittedObject.OnWithdrew.RemoveAllListeners();
            laser.HittedObject.OnWithdrew.AddListener(OnWithdrawCallback);
            laser.HittedObject.Withdraw();
            curState = WithdrawerState.WAIT;
            InputManager.Instance.pause = true;
            OnWithdraw.Invoke();
        }

        public void OnWithdrawCallback()
        {
            holdColor = withdrawColor;
            laser.Color = holdColor;
            withdrawContainer.color = Dimension.MaterialColor[holdColor];
            curState = WithdrawerState.TO_INSERT;
            if (laser.HittedObject.Color == Dimension.Color.NONE)
            {
                Splittable.SplittableObject so;
                if (laser.HittedObject.IsRoot)
                    so = laser.HittedObject.GetComponent<Splittable.SplittableObject>();
                else
                    so = laser.HittedObject.Root.GetComponent<Splittable.SplittableObject>();
                World.Instance.DeactivateObject(so);
            }
            laser.HittedObject = null;
            TurnOff();
            InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Inserts the <c>holdColor</c> to the <c>SplittableObject</c>
        /// hitted by the laser.
        /// </summary>
        private void Insert()
        {
            laser.HittedObject.SecondColor = laser.HittedObject.Color;
            laser.HittedObject.Color = Dimension.AddColor(holdColor, laser.HittedObject.Color);
            laser.HittedObject.ContactPoint = laser.ContactPoint;
            laser.HittedObject.OnInserted.RemoveAllListeners();
            laser.HittedObject.OnInserted.AddListener(OnInsertCallback);
            laser.HittedObject.Insert();
            curState = WithdrawerState.WAIT;
            withdrawContainer.color = transparentColor;
            InputManager.Instance.pause = true;
            OnInsert.Invoke();
        }

        public void OnInsertCallback()
        {
            holdColor = Dimension.Color.NONE;
            laser.HittedObject = null;
            TurnOff();
            hintSide.color = Dimension.MaterialColor[Dimension.Color.WHITE];
            InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Checks and updates if the user changes the skill color.
        /// </summary>
        public void RotateSkillColor()
        {
            if (curState != WithdrawerState.TO_WITHDRAW)
                return;
            withdrawColorIdx = (withdrawColorIdx + 1) % Dimension.BaseColor.Count;
            withdrawColor = Dimension.BaseColor[withdrawColorIdx];
            laser.Color = withdrawColor;
            var zAngle = (colorSelector.eulerAngles.z + 120) % 360;
            colorSelector.eulerAngles = new Vector3(0, 0, zAngle);
            hintSide.color = Dimension.MaterialColor[withdrawColor];
        }

    }

}