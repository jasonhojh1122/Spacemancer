using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
public class SkillController : MonoBehaviour
{
    [SerializeField] SkillGameUIController skillGameUIController;
    public Dimension.Color skillColor = Dimension.Color.NONE;
    private Collider hitCollider = null;
    enum SkillMode
    {
        WITHDRAW,FILL
    }
    private void FixedUpdate()
    {
        if (Input.GetButton("ChooseColor"))
        {
            Debug.Log("Choosing Color");
            skillGameUIController.gameObject.SetActive(true);
        }
        else if (Input.GetButtonDown("ChooseRed"))
        {
            Debug.Log("Choose RED");
            skillGameUIController.gameObject.SetActive(true);
            skillGameUIController.RedButton.Select();
        }
        else if (Input.GetButtonDown("ChooseGreen"))
        {
            Debug.Log("Choose GREEN");
            skillGameUIController.gameObject.SetActive(true);
            skillGameUIController.GreenButton.Select();
        }
        else if (Input.GetButtonDown("ChooseBlue"))
        {
            Debug.Log("Choose BLUE");
            skillGameUIController.gameObject.SetActive(true);
            skillGameUIController.BlueButton.Select();
        }
        skillGameUIController.gameObject.SetActive(false);
    }
    private void withdraw()
    {
        ObjectColor color = hitCollider.GetComponent<ObjectColor>();
        if (tryColor(color.Color,skillColor,SkillMode.WITHDRAW))
        {

        }
    }
    private void fill()
    {
        ObjectColor color = hitCollider.GetComponent<ObjectColor>();
        if (tryColor(color.Color, skillColor, SkillMode.FILL))
        {

        }
    }
    private bool tryColor(Dimension.Color objectColor,Dimension.Color targetColor,SkillMode mode)
    {
        bool ret = true;
        Dimension.Color tmp;
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
