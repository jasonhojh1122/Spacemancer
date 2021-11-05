using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
//activate obj when desired object enter zone
public class Button : MonoBehaviour
{
    
    [SerializeField] GameObject toggleObject;
    [SerializeField] string desiredObjectTag;
    [SerializeField] Dimension.Color desiredObjectColor; //set none if no restriction
    [SerializeField] bool isActive = false;

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
        if (match(other.gameObject))
        {
            toggleObject.SetActive(!isActive);
            isActive = !isActive;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("button Zone Exit");
        if (match(other.gameObject))
        {
            toggleObject.SetActive(!isActive);
            isActive = !isActive;
        }
    }

}
