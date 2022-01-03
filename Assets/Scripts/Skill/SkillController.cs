using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UI;

namespace Skill
{
    public class SkillController : MonoBehaviour
    {
        public enum SkillState
        {
            OFF, TO_WITHDRAW, TO_Insert, WAIT
        }
        [SerializeField] Laser laser;
        [SerializeField] UI.SkillControllerUI skillControllerUI;
        [SerializeField] CanvasGroupFader skillControllerUIFader;
        [SerializeField, Range(0, 20)] int usedLimit;

        /// <summary>
        /// Current color for withdrawing.
        /// </summary>
        Dimension.Color withdrawColor;

        /// <summary>
        /// Withdrew color held by the skill.
        /// </summary>
        Dimension.Color holdColor;
        int colorIdx = 0;
        float lastPressedLT = 0.0f;
        float lastPressedRT = 0.0f;
        float betweenPressed = 0.5f;
        int usedCount = 0;
        SkillState curState;

        /// <summary>
        /// Current state of skill.
        /// </summary>
        public SkillState CurState
        {
            get => curState;
        }

        private void Awake()
        {
            curState = SkillState.OFF;
            withdrawColor = Dimension.BaseColor[0];
            holdColor = Dimension.Color.NONE;
            skillControllerUI.Init(usedLimit);
        }

        private void Update()
        {
            if (InputManager.Instance.pause || curState == SkillState.WAIT)
            {
                return;
            }

            if (Input.GetButtonDown("Skill") && (usedCount < usedLimit))
            {
                UpdateSkillState();
            }

            if (curState == SkillState.TO_WITHDRAW)
            {
                CheckSkillColorChange();
            }
        }

        /// <summary>
        /// Checks and updates if the user changes the skill color.
        /// </summary>
        private void CheckSkillColorChange()
        {
            bool changed = false;
            if ( Input.GetButtonDown("NextColor") ||
                (Input.GetAxis("NextColorJoystick") >= 1 && (Time.time - lastPressedRT > betweenPressed)) )
            {
                lastPressedRT = Time.time;
                colorIdx = (colorIdx + 1) % Dimension.BaseColor.Count;
                changed = true;
            }
            else if ( Input.GetButtonDown("PreviousColor") ||
                    (Input.GetAxis("PreviousColorJoystick") >= 1 && (Time.time - lastPressedLT > betweenPressed)) )
            {
                lastPressedLT = Time.time;
                colorIdx = (colorIdx + Dimension.BaseColor.Count - 1) % Dimension.BaseColor.Count;
                changed = true;
            }

            if (changed)
            {
                withdrawColor = Dimension.BaseColor[colorIdx];
                laser.Color = withdrawColor;
                skillControllerUI.Select(withdrawColor);
            }
        }

        /// <summary>
        /// Updates the skill state.
        /// </summary>
        private void UpdateSkillState()
        {
            switch (curState)
            {
                case SkillState.OFF: // Turns on the skill and fade in the UI.
                    laser.IsOn = true;
                    skillControllerUIFader.FadeIn();
                    if (holdColor != Dimension.Color.NONE)
                    {
                        withdrawColor = holdColor;
                        curState = SkillState.TO_Insert;
                        skillControllerUI.Hold(holdColor);
                    }
                    else
                    {
                        curState = SkillState.TO_WITHDRAW;
                        skillControllerUI.UnMaskAll();
                        skillControllerUI.Select(withdrawColor);
                    }
                    laser.Color = withdrawColor;
                    break;
                case SkillState.TO_WITHDRAW: // Try to withdraw the color.
                    if (laser.HittedObject != null && !laser.HittedObject.IsPersistentColor &&
                        ((laser.HittedObject.Color & withdrawColor) != Dimension.Color.NONE) )
                        Withdraw();
                    else
                        TurnOffSkill();
                    break;
                case SkillState.TO_Insert: // Try to insert the held color.
                    if (laser.HittedObject != null && !laser.HittedObject.IsPersistentColor &&
                        ((laser.HittedObject.Color & holdColor) == Dimension.Color.NONE) )
                        Insert();
                    else
                        TurnOffSkill();
                    break;
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
            oc.OnWithdrew.AddListener(OnWithdrewCallback);
            oc.Withdraw(laser.ContactPoint);
            curState = SkillState.WAIT;
            InputManager.Instance.pause = true;
        }

        public void OnWithdrewCallback()
        {
            holdColor = withdrawColor;
            curState = SkillState.TO_Insert;
            skillControllerUI.Hold(holdColor);
            laser.Color = holdColor;
            if (laser.HittedObject.Color == Dimension.Color.NONE)
            {
                World.Instance.DeactivateObject(laser.HittedObject);
                laser.HittedObject = null;
            }
            TurnOffSkill();
            InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Inserts the <c>holdColor</c> to the <c>SplittableObject</c>
        /// hitted by the laser.
        /// </summary>
        private void Insert()
        {
            var oc = laser.HittedObject.ObjectColor;
            oc.SecondColor = Dimension.AddColor(holdColor, oc.Color);
            oc.OnInserted.RemoveAllListeners();
            oc.OnInserted.AddListener(OnInsertCallback);
            oc.Insert(laser.ContactPoint);
            curState = SkillState.WAIT;
            InputManager.Instance.pause = true;
        }

        public void OnInsertCallback()
        {
            laser.HittedObject.ObjectColor.Color = laser.HittedObject.ObjectColor.SecondColor;
            holdColor = Dimension.Color.NONE;
            usedCount++;
            skillControllerUI.Sub();
            TurnOffSkill();
            InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Turns off the skill.
        /// </summary>
        private void TurnOffSkill()
        {
            if (laser.HittedObject != null)
            {
                laser.HittedObject.ObjectColor.Reset();
            }
            laser.IsOn = false;
            skillControllerUIFader.FadeOut();
            curState = SkillState.OFF;
        }
    }

}