using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
//activate obj when desired object enter zone

using Core;

[RequireComponent(typeof(Core.SplittableObject))]
public class Button : MonoBehaviour
{

    [SerializeField] Core.SplittableObject toggleObject;
    [SerializeField] string targetObjectName;
    [SerializeField] string desiredObjectName;
    // [SerializeField] bool isTriggered = false;

    World world;
    Core.SplittableObject so;

    private void Awake()
    {
        so = GetComponent<Core.SplittableObject>();
        world = FindObjectOfType<World>();
    }

    private void Start()
    {
        // world.SetObjectActive(toggleObject, isTriggered);
        // toggleObject.gameObject.SetActive(isTriggered);
    }

    private bool Match(GameObject obj)
    {
        var objSo = obj.GetComponent<Core.SplittableObject>();
        if (objSo.ObjectColor.Color == Dimension.Color.NONE || objSo.ObjectColor.Color != so.ObjectColor.Color)
        {
            return false;
        }
        return obj.name == desiredObjectName;
    }

    private void ToggleOn()
    {
        var set = world.ObjectPool.InactiveObjects.Pool[targetObjectName];
        if (set == null) return;
        List<SplittableObject> toActivate = new List<SplittableObject>();
        foreach (SplittableObject obj in set)
        {
            if (obj.ObjectColor.Color == so.ObjectColor.Color)
            {
                toActivate.Add(obj);
            }
        }
        foreach (SplittableObject obj in toActivate)
        {
            world.ActivateObject(obj, so.ObjectColor.Color);
        }
    }

    private void ToggleOff()
    {
        var set = world.ObjectPool.ActiveObjects.Pool[targetObjectName];
        if (set == null) return;
        List<SplittableObject> toInactivate = new List<SplittableObject>();
        foreach (SplittableObject obj in set)
        {
            if (obj.ObjectColor.Color == so.ObjectColor.Color)
            {
                world.DeleteObject(obj);
                toInactivate.Add(obj);
            }
        }
        foreach (SplittableObject obj in toInactivate)
        {
            world.DeleteObject(obj);
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("button Zone enter");
        if (!so.IsInCorrectDim())
            return;
        if (Match(other.gameObject))
        {
            Debug.Log("matched");
            ToggleOn();
        }
    }

    private void OnCollisionExit(Collision other) {
        Debug.Log("button Zone Exit");
        if (!so.IsInCorrectDim())
            return;

        if (Match(other.gameObject))
        {
            Debug.Log("matched");
            ToggleOff();
        }
    }

    /* void OnTriggerEnter(Collider other)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<SplittableObject>().ObjectColor != activeColor)
            return;

        if (match(other.gameObject))
        {
            toggleObject.SetActive(!isTriggered);
            isTriggered = !isTriggered;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<SplittableObject>().ObjectColor != activeColor)
            return;

        Debug.Log("button Zone Exit");
        if (match(other.gameObject))
        {
            toggleObject.SetActive(!isTriggered);
            isTriggered = !isTriggered;
        }
    } */

}
