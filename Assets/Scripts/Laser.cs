using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
public class Laser : MonoBehaviour
{
    LineRenderer lr;
    public Collider hitCollider;
    private bool _laserIsOn = false;
    [SerializeField] List<Material> material;
    Vector3 y_Offset = new Vector3(0, 0.2f, 0);
    public Vector3 forward_Offset;
    public bool laserIsOn
    {
        get => _laserIsOn;
        set
        {
            _laserIsOn = value;
            if(value == false)
            {
                lr.enabled = false;
                hitCollider = null;
                forward_Offset = new Vector3(0, 0, 0);
            }
            else
            {
                lr.enabled = true;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        forward_Offset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<SkillController>().alreadyFilled)
        {
            lr.material = material[1];
        }
        else
        {
            lr.material = material[0];
        }
        if (laserIsOn)
        {
            Debug.Log("Laser Is On");
            lr.startColor = dimToColor();
            lr.endColor = lr.startColor;
            lr.material.color = lr.startColor;
            lr.SetPosition(0, transform.position+y_Offset);
            RaycastHit hit;
            if (Physics.Raycast(transform.position+y_Offset, (transform.forward+forward_Offset).normalized, out hit))
            {
                if (hit.collider)
                {
                    lr.SetPosition(1, hit.point);
                }
                hitCollider = hit.collider;
            }
            else lr.SetPosition(1, (transform.forward+forward_Offset).normalized * 5000);

        }

    }
    Color dimToColor()
    {
        Color ret = Color.clear;
        switch (GetComponent<SkillController>().skillColor)
        {
            case Dimension.Color.RED:
                ret = Color.red;
                break;
            case Dimension.Color.BLUE:
                ret = Color.blue;
                break;
            case Dimension.Color.GREEN:
                ret = Color.green;
                break;
            default:
                ret = Color.clear;
                break;
        }
        return ret;
    }
}

