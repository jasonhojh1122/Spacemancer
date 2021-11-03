using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxes : MonoBehaviour,Interaction
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public bool Interact()
    {
        if (interactable || isPickUp)
        {
            PickupEvent();
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool interactable
    {
        set => _interactable = value;
        get => _interactable;
    }
    bool _interactable;
    public bool fallingDown = false;
    private bool isPickUp = false;
    GameObject possibleParent;
    public void PickupEvent()
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
        interactable = true;
        rb.isKinematic = true;
        Vector3 localPos = transform.localPosition;
        Quaternion localRot = transform.localRotation;
        transform.SetParent(possibleParent.transform);
        transform.localPosition = localPos;
        transform.localRotation = localRot;
        transform.localPosition = new Vector3(0, 3, 0);
        Debug.Log("Picked Up");
    }
    void PutDown()
    {
        rb.isKinematic = false;
        isPickUp = false;
        transform.SetParent(transform.parent.transform.parent);
        Debug.Log("Put Down");
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.z));
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            interactable = true;
            possibleParent = other.gameObject;
            Debug.Log("Player Enter Box");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            interactable = false;
            possibleParent = null;
        }
    }
    // Start is called before the first frame update
}
