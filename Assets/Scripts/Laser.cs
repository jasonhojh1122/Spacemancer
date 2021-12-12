using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [SerializeField] float width;

    LineRenderer lr;
    Vector3 y_Offset = new Vector3(0, 0.2f, 0);
    MaterialPropertyBlock _property;
    SplittableObject hittedObject;
    Dimension.Color _color;
    bool _IsOn = false;
    Vector3 lastContactPoint;
    public Dimension.Color Color
    {
        get => _color;
        set
        {
            _color = value;
            lr.GetPropertyBlock(_property);
            _property.SetColor("_Color", Dimension.MaterialColor[value]);
            lr.SetPropertyBlock(_property);
            if (hittedObject != null)
            {
                hittedObject.ObjectColor.SelectColor = _color;
            }
        }
    }
    public bool IsOn
    {
        get => _IsOn;
        set
        {
            _IsOn = value;
            if(value == false)
            {
                lr.enabled = false;
                if (hittedObject != null)
                {
                    hittedObject.ObjectColor.SkillUnselect(lastContactPoint);
                    hittedObject = null;
                }
            }
            else
            {
                lr.enabled = true;
            }
        }
    }
    public SplittableObject HittedObject
    {
        get => hittedObject;
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = width;
        lr.endWidth = width;
        _property = new MaterialPropertyBlock();
    }

    void FixedUpdate()
    {
        if (IsOn)
        {
            lr.SetPosition(0, transform.position + y_Offset);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + y_Offset, transform.forward.normalized, out hit) &&
                hit.collider != null)
            {
                lr.SetPosition(1, hit.point);
                var newHittedObject = hit.collider.gameObject.GetComponent<SplittableObject>();
                UpdateObjectMaterial(hit.point, newHittedObject);
            }
            else
            {
                lr.SetPosition(1, transform.forward.normalized * 5000);
                if (hittedObject != null && hittedObject.gameObject.activeSelf)
                {
                    hittedObject.ObjectColor.SkillUnselect(lastContactPoint);
                }
                hittedObject = null;
            }
        }

    }

    void UpdateObjectMaterial(Vector3 contactPoint, SplittableObject newHittedObject)
    {
        bool selectNew = false;
        bool unselectOld = false;
        if (newHittedObject == null)
        {
            if (hittedObject != null)
            {
                unselectOld = true;
            }
        }
        else
        {
            if (hittedObject == null)
            {
                selectNew = true;
            }
            else if (hittedObject.gameObject.GetInstanceID() != newHittedObject.gameObject.GetInstanceID())
            {
                selectNew = true;
                unselectOld = true;
            }
        }

        if (unselectOld)
        {
            hittedObject.ObjectColor.SkillUnselect(lastContactPoint);
        }
        hittedObject = newHittedObject;
        if (selectNew)
        {
            if (hittedObject.IsPersistentColor)
            {
                hittedObject = null;
            }
            else
            {
                hittedObject.ObjectColor.SelectColor = _color;
                hittedObject.ObjectColor.SkillSelect(contactPoint);
            }
        }
        lastContactPoint = contactPoint;
    }

}

