using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
public class SkillController : MonoBehaviour
{
    public Dimension.Color skillColor = Dimension.Color.NONE;
    private Collider hitCollider = null;
    public bool alreadyFilled = false;
    int colorIdx = 0;
    float lastPressedLT=0.0f;
    float lastPressedRT = 0.0f;
    float betweenPressed = 0.5f;
    List<Dimension.Color> colorList =new List<Dimension.Color>{Dimension.Color.RED,Dimension.Color.GREEN,Dimension.Color.BLUE};
    enum SkillMode
    {
        WITHDRAW,FILL
    }
    private void Start()
    {
        skillColor = Dimension.Color.RED;
    }
    private void Update()
    {
        hitCollider = GetComponent<Laser>().hitCollider;
        if (Input.GetButtonDown("NextColor")|| (Input.GetAxis("NextColorJoystick") >=1&& (Time.time-lastPressedRT > betweenPressed))&& !alreadyFilled)
        {
            lastPressedRT = Time.time;
            colorIdx = (colorIdx + 1) % colorList.Count;
            skillColor = colorList[colorIdx];
        }
        else if (Input.GetButtonDown("PreviousColor")||(Input.GetAxis("PreviousColorJoystick")>=1 && (Time.time - lastPressedLT > betweenPressed)) && !alreadyFilled)
        {
            lastPressedLT = Time.time;
            colorIdx = (colorIdx + colorList.Count - 1) % colorList.Count;
            skillColor = colorList[colorIdx];
        }
//
  //      if (GetComponent<Laser>().laserIsOn)
    //    {
      //      float laser_Y = Input.GetAxisRaw("LaserControlV");
        //    GetComponent<Laser>().forward_Offset.y += laser_Y*0.01f;
        //}
        if (Input.GetButtonDown("Skill"))
        {
            Debug.Log("Skill Pressed");
            Skill();
        }
    }
    private void Skill()
    {
        GetComponent<Laser>().laserIsOn = true;
        if (hitCollider == null)
        {
            return;
        }
        if (alreadyFilled)
        {
            fill();
        }
        else
        {
            withdraw();
        }
    }
    private void withdraw()
    {
        ObjectColor color = hitCollider.GetComponent<ObjectColor>();
        if (tryColor(color.Color,skillColor,SkillMode.WITHDRAW))
        {
            color.Color = color.Color ^ skillColor;
            alreadyFilled = true;
            Debug.Log("Withdrawn");
            hitCollider = null;
        }
    }
    private void fill()
    {
        ObjectColor color = hitCollider.GetComponent<ObjectColor>();
        if (tryColor(color.Color, skillColor, SkillMode.FILL))
        {
            color.Color = color.Color | skillColor;
            alreadyFilled = false;
            GetComponent<Laser>().laserIsOn = false;
            Debug.Log("Filled");
            hitCollider = null;
        }
    }
    private bool tryColor(Dimension.Color objectColor,Dimension.Color targetColor,SkillMode mode)
    {
        bool ret = true;
        Dimension.Color tmp;
        Debug.Log("Trying Color");
        switch (mode)
        {
            case SkillMode.WITHDRAW:// withdraw mode
                tmp = objectColor & targetColor;
                if(tmp!=targetColor)
                {
                    ret = false;
                }
                break;
            case SkillMode.FILL: // fill mode
            default:
                tmp = objectColor | targetColor;
                if(tmp == objectColor)
                {
                    ret = false;
                }
                break;
        }
        return ret;
    }
}
