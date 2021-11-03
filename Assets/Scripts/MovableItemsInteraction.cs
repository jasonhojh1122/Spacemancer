using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableItemsInteraction : MonoBehaviour
{
    Rigidbody rb;
    public bool fallingDown = false;
    public bool isPickUp = false;
    public bool canPickUp = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "Floor")
        {
            return;
        }
        if (other.gameObject.tag == "Player")
            Debug.Log("Player Enter");
    }
    private void OnTriggerExit(Collider other)
    {
    }
}
