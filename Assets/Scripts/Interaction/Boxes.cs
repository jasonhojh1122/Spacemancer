using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction {
public class Boxes : Interactable
{
    [SerializeField] Vector3 pickUpOffset;
    Rigidbody rb;
    private bool isPickUp = false;
    Transform player;

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
        transform.SetParent(player);
        transform.position = pos + pickUpOffset;
        transform.rotation = rot;
        Debug.Log("Picked Up");
    }

    void PutDown()
    {
        Debug.Log("Put Down");
        rb.isKinematic = false;
        isPickUp = false;
        transform.SetParent(player.parent.transform.parent);
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.z));
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public override void OnZoneEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            player = other.gameObject.transform;
            Debug.Log("Player Enter Box");
        }
    }
    public override void OnZoneStay(Collider other) {
        return;
    }

    public override void OnZoneExit(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            player = null;
        }
    }

    public override bool IsInteracting() {
        return isPickUp;
    }
}
}