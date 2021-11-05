using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
//activate obj when desired object enter zone

[RequireComponent(typeof(SplitableObject))]
public class Button : MonoBehaviour
{
    
    [SerializeField] GameObject toggleObject;
    [SerializeField] string desiredObjectTag;
    [SerializeField] Dimension.Color desiredObjectColor; //set none if no restriction
    [SerializeField] Dimension.Color activeColor;        //should be same as desired color(?
    [SerializeField] bool isTriggered = false;

    private bool match(GameObject obj)
    {

        if (desiredObjectColor != Dimension.Color.NONE)
        {
            if (obj.GetComponent<SplitableObject>() == null || obj.GetComponent<SplitableObject>().ObjectColor != desiredObjectColor) 
            {
                return false;
            }
        }
        return obj.tag == desiredObjectTag;

    }

    void OnTriggerEnter(Collider other)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<SplitableObject>().ObjectColor != activeColor)
            return;

        if (match(other.gameObject))
        {
            toggleObject.SetActive(!isTriggered);
            isTriggered = !isTriggered;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<SplitableObject>().ObjectColor != activeColor)
            return;

        Debug.Log("button Zone Exit");
        if (match(other.gameObject))
        {
            toggleObject.SetActive(!isTriggered);
            isTriggered = !isTriggered;
        }
    }

}
