using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction {
public class Boxes : Interactable
{
    Rigidbody rb;
    private bool isPickUp = false;
    GameObject possibleParent;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Interact()
    {
        if (!isPickUp)
        {
            PickUp();
        }
        else
        {
            PutDown();
        }
    }


    void PickUp()
    {
        isPickUp = true;
        rb.isKinematic = true;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        transform.SetParent(possibleParent.transform);
        transform.position = pos;
        transform.rotation = rot;
        // transform.localPosition = new Vector3(0, 3, 0);
        Debug.Log("Picked Up");
    }
    void PutDown()
    {
        Debug.Log("Put Down");
        rb.isKinematic = false;
        isPickUp = false;
        transform.SetParent(transform.parent.transform.parent);
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.z));
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public override void OnZoneEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            possibleParent = other.gameObject;
            Debug.Log("Player Enter Box");
        }
    }
    public override void OnZoneStay(Collider other) {
        return;
    }

    public override void OnZoneExit(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            possibleParent = null;
        }
    }
}
}