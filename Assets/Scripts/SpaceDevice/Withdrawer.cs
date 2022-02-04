
using UnityEngine;
using UnityEngine.InputSystem;

using Core;

namespace SpaceDevice
{
    public class Withdrawer : InputController
    {
        enum WithdrawerState
        {
            OFF, TO_WITHDRAW, TO_INSERT, WAIT
        }
        [SerializeField] Laser laser;
        [SerializeField] EnergyBar energyBar;
        [SerializeField] RectTransform colorSelector;
        [SerializeField] UnityEngine.UI.Image withdrawContainer;
        [SerializeField] float energyCost = 0.2f;

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

        new void Awake()
        {
            base.Awake();
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
            if (curState == WithdrawerState.WAIT) return;
            if (curState == WithdrawerState.OFF)
                TurnOn();
            else
                TurnOff();
        }

        void TurnOn()
        {
            laser.IsOn = true;
            if (holdColor != Dimension.Color.NONE)
                curState = WithdrawerState.TO_INSERT;
            else
                curState = WithdrawerState.TO_WITHDRAW;
            laser.Color = withdrawColor;
        }

        void TurnOff()
        {
            if (laser.HittedObject != null)
                laser.HittedObject.ObjectColor.Reset();
            laser.IsOn = false;
            curState = WithdrawerState.OFF;
        }

        public void Perform()
        {
            if (curState == WithdrawerState.OFF)
                return;
            else if (curState == WithdrawerState.TO_WITHDRAW) // Try to withdraw the color.
            {
                if (laser.HittedObject != null && !laser.HittedObject.IsPersistentColor &&
                    ((laser.HittedObject.Color & withdrawColor) != Dimension.Color.NONE) )
                    Withdraw();
                else
                    TurnOff();
            }
            else if (curState == WithdrawerState.TO_INSERT)
            {
                if (laser.HittedObject != null && !laser.HittedObject.IsPersistentColor &&
                    ((laser.HittedObject.Color & holdColor) == Dimension.Color.NONE) )
                    Insert();
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
            var oc = laser.HittedObject.ObjectColor;
            oc.SecondColor = oc.Color;
            oc.Color = Dimension.SubColor(oc.Color, withdrawColor);
            oc.OnWithdrew.RemoveAllListeners();
            oc.OnWithdrew.AddListener(OnWithdrawCallback);
            oc.Withdraw(laser.ContactPoint);
            curState = WithdrawerState.WAIT;
            InputManager.Instance.pause = true;
        }

        public void OnWithdrawCallback()
        {
            holdColor = withdrawColor;
            curState = WithdrawerState.TO_INSERT;
            laser.Color = holdColor;
            withdrawContainer.color = Dimension.MaterialColor[holdColor];
            if (laser.HittedObject.Color == Dimension.Color.NONE)
            {
                World.Instance.DeactivateObject(laser.HittedObject);
                laser.HittedObject = null;
            }
            TurnOff();
            InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Inserts the <c>holdColor</c> to the <c>SplittableObject</c>
        /// hitted by the laser.
        /// </summary>
        private void Insert()
        {
            Debug.Log("INSERT");
            var oc = laser.HittedObject.ObjectColor;
            oc.SecondColor = Dimension.AddColor(holdColor, oc.Color);
            oc.OnInserted.RemoveAllListeners();
            oc.OnInserted.AddListener(OnInsertCallback);
            oc.Insert(laser.ContactPoint);
            curState = WithdrawerState.WAIT;
            withdrawContainer.color = transparentColor;
            energyBar.AddEnergy(-energyCost);
            InputManager.Instance.pause = true;
        }

        public void OnInsertCallback()
        {
            laser.HittedObject.ObjectColor.Color = laser.HittedObject.ObjectColor.SecondColor;
            holdColor = Dimension.Color.NONE;
            TurnOff();
            InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Checks and updates if the user changes the skill color.
        /// </summary>
        public void RotateSkillColor()
        {
            withdrawColorIdx = (withdrawColorIdx + 1) % Dimension.BaseColor.Count;
            withdrawColor = Dimension.BaseColor[withdrawColorIdx];
            laser.Color = withdrawColor;
            var zAngle = (colorSelector.eulerAngles.z + 120) % 360;
            colorSelector.eulerAngles = new Vector3(0, 0, zAngle);
        }
    }

}