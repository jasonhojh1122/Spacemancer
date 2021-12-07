using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class SkillController : MonoBehaviour
{
    public enum SkillState
    {
        OFF, TO_WITHDRAW, TO_FILL
    }
    [SerializeField] Laser laser;
    [SerializeField] SkillControllerUI skillControllerUI;
    [SerializeField] CanvasGroupFader skillControllerUIFader;

    Dimension.Color selectionColor;
    Dimension.Color holdColor;
    int colorIdx = 0;
    float lastPressedLT = 0.0f;
    float lastPressedRT = 0.0f;
    float betweenPressed = 0.5f;
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
    }

    private void Update()
    {
        if (Input.GetButtonDown("Skill"))
        {
            Debug.Log("Skill Pressed");
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
                    curState = SkillState.TO_FILL;
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
                if (laser.HittedObject != null)
                    Withdraw();
                else
                    TurnOffSkill();
                break;
            case SkillState.TO_FILL:
                if (laser.HittedObject != null)
                    Fill();
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
        laser.HittedObject.Color = laser.HittedObject.Color ^ selectionColor;
        holdColor = selectionColor;
        curState = SkillState.TO_FILL;
        skillControllerUI.Hold(holdColor);
    }

    private void Fill()
    {
        if (laser.HittedObject.IsPersistentColor ||
            (laser.HittedObject.Color & holdColor) != Dimension.Color.NONE)
        {
            return;
        }
        laser.HittedObject.Color = laser.HittedObject.Color | holdColor;
        holdColor = Dimension.Color.NONE;
        TurnOffSkill();
    }

    private void TurnOffSkill()
    {
        laser.IsOn = false;
        skillControllerUIFader.FadeOut();
        curState = SkillState.OFF;
    }
}
