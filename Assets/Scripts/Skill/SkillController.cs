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
        [SerializeField] int laserLimit; //no limit = -1
        Dimension.Color selectionColor;
        Dimension.Color holdColor;
        int colorIdx = 0;
        float lastPressedLT = 0.0f;
        float lastPressedRT = 0.0f;
        float betweenPressed = 0.5f;
        int laserCount = 0;
        SkillState curState;
        public SkillState CurState
        {
            get => curState;
        }
        private void Awake()
        {
            curState = SkillState.OFF;
            selectionColor = Dimension.BaseColor[0];
            holdColor = Dimension.Color.NONE;
            skillControllerUI.Init(laserLimit);
        }

        private void Update()
        {
            if (curState == SkillState.WAIT) return;
            if(InputManager.Instance.pause)  return;
            if (Input.GetButtonDown("Skill") )
            {
                if(laserCount < laserLimit || laserLimit == -1)
                    Skill();
            }

            // color can be change before withdrawing
            if (curState == SkillState.TO_WITHDRAW)
            {
                if ( Input.GetButtonDown("NextColor") ||
                    (Input.GetAxis("NextColorJoystick") >= 1 && (Time.time - lastPressedRT > betweenPressed)) )
                {
                    lastPressedRT = Time.time;
                    colorIdx = (colorIdx + 1) % Dimension.BaseColor.Count;
                    selectionColor = Dimension.BaseColor[colorIdx];
                    laser.Color = selectionColor;
                    skillControllerUI.Select(selectionColor);
                }
                else if ( Input.GetButtonDown("PreviousColor") ||
                        (Input.GetAxis("PreviousColorJoystick") >= 1 && (Time.time - lastPressedLT > betweenPressed)) )
                {
                    lastPressedLT = Time.time;
                    colorIdx = (colorIdx + Dimension.BaseColor.Count - 1) % Dimension.BaseColor.Count;
                    selectionColor = Dimension.BaseColor[colorIdx];
                    laser.Color = selectionColor;
                    skillControllerUI.Select(selectionColor);
                }
            }
        }

        private void Skill()
        {
            switch (curState)
            {
                case SkillState.OFF:
                    laser.IsOn = true;
                    skillControllerUIFader.FadeIn();
                    if (holdColor != Dimension.Color.NONE)
                    {
                        selectionColor = holdColor;
                        curState = SkillState.TO_Insert;
                        skillControllerUI.Hold(holdColor);
                    }
                    else
                    {
                        curState = SkillState.TO_WITHDRAW;
                        skillControllerUI.UnMaskAll();
                        skillControllerUI.Select(selectionColor);
                    }
                    laser.Color = selectionColor;
                    break;
                case SkillState.TO_WITHDRAW:
                    if (laser.HittedObject != null && (laserCount < laserLimit || laserLimit == -1))
                        Withdraw();
                    else
                        TurnOffSkill();
                    break;
                case SkillState.TO_Insert:
                    if (laser.HittedObject != null)
                        Insert();
                    else
                        TurnOffSkill();
                    break;
            }
        }

        private void Withdraw()
        {
            if (laser.HittedObject.IsPersistentColor ||
                ((laser.HittedObject.Color & selectionColor) == Dimension.Color.NONE) )
            {
                return;
            }
            var oc = laser.HittedObject.ObjectColor;
            oc.SecondColor = oc.Color;
            oc.Color = Dimension.SubColor(oc.Color, selectionColor);
            oc.OnWithdrew.RemoveAllListeners();
            oc.OnWithdrew.AddListener(OnWithdrew);
            oc.Withdraw(laser.ContactPoint);
            curState = SkillState.WAIT;
        }

        public void OnWithdrew()
        {
            holdColor = selectionColor;
            curState = SkillState.TO_Insert;
            skillControllerUI.Hold(holdColor);
            laser.Color = holdColor;
            if (laser.HittedObject.Color == Dimension.Color.NONE)
            {
                World.Instance.DeactivateObject(laser.HittedObject);
                laser.HittedObject = null;
            }
            TurnOffSkill();
        }

        private void Insert()
        {
            if (laser.HittedObject.IsPersistentColor ||
                (laser.HittedObject.Color & holdColor) != Dimension.Color.NONE)
            {
                return;
            }
            var oc = laser.HittedObject.ObjectColor;
            oc.SecondColor = Dimension.AddColor(holdColor, oc.Color);
            oc.OnInserted.RemoveAllListeners();
            oc.OnInserted.AddListener(OnInsert);
            oc.Insert(laser.ContactPoint);
            curState = SkillState.WAIT;
        }

        public void OnInsert()
        {
            laser.HittedObject.ObjectColor.Color = laser.HittedObject.ObjectColor.SecondColor;
            holdColor = Dimension.Color.NONE;
            laserCount++;
            skillControllerUI.Sub();
            TurnOffSkill();
        }

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