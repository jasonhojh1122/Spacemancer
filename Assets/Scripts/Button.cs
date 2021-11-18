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
    [SerializeField] string desiredObjectTag;
    [SerializeField] Dimension.Color desiredObjectColor; //set none if no restriction
    [SerializeField] Dimension.Color activeColor;        //should be same as desired color(?
    [SerializeField] bool isTriggered = false;

    Core.SplittableObject so;

    private void Awake() {
        so = GetComponent<Core.SplittableObject>();
        toggleObject.gameObject.SetActive(isTriggered);
    }

    private bool match(GameObject obj)
    {
        if (desiredObjectColor != Dimension.Color.NONE)
        {
            if (obj.GetComponent<Core.SplittableObject>() == null || obj.GetComponent<Core.SplittableObject>().ObjectColor.Color != desiredObjectColor)
            {
                return false;
            }
        }
        return obj.tag == desiredObjectTag;

    }

    private void OnCollisionEnter(Collision other) {
        if (activeColor != Dimension.Color.NONE && so.ObjectColor.Color != activeColor)
            return;

        if (match(other.gameObject))
        {
            toggleObject.gameObject.SetActive(!isTriggered);
            isTriggered = !isTriggered;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (activeColor != Dimension.Color.NONE && GetComponent<Core.SplittableObject>().ObjectColor.Color != activeColor)
            return;

        Debug.Log("button Zone Exit");
        if (match(other.gameObject))
        {
            toggleObject.gameObject.SetActive(!isTriggered);
            isTriggered = !isTriggered;
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
