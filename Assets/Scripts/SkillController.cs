using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
public class SkillController : MonoBehaviour
{
    [SerializeField] SkillGameUIController skillGameUIController;
    public Dimension.Color skillColor = Dimension.Color.NONE;
    private Collider hitCollider = null;
    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Skill"))
        {
            skillGameUIController.gameObject.SetActive(true);
        }
        skillGameUIController.gameObject.SetActive(false);
    }
    private void withdraw()
    {
        ObjectColor color = hitCollider.GetComponent<ObjectColor>();
        if (tryColor(color.Color,skillColor,0))
        {

        }
    }
    private void fill()
    {
        ObjectColor color = hitCollider.GetComponent<ObjectColor>();
        if (tryColor(color.Color, skillColor, 1))
        {

        }
    }
    private bool tryColor(Dimension.Color objectColor,Dimension.Color targetColor,int mode)
    {
        bool ret = true;
        Dimension.Color tmp;
        switch (mode)
        {
            case 0:// withdraw mode
                tmp = objectColor & targetColor;
                if(tmp!=targetColor)
                {
                    ret = false;
                }
                break;
            case 1: // fill mode
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
