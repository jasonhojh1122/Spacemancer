using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;

namespace Interaction {
    [RequireComponent(typeof(Core.SplittableObject))]
    public class Box : Interactable
    {
        [SerializeField] Dimension.Color activeColor; // None if no restriction
        [SerializeField] float pickUpOffset = 0.1f;
        Rigidbody rb;
        public static Box pickUpBox = null;
        Transform player;
        bool playerInZone;

        void Start()
        {
            player = FindObjectsOfType<Character.PlayerController>()[0].transform;
            rb = GetComponent<Rigidbody>();
        }
        /*void Update()
        {
            if (Input.GetButtonDown("Interact"))
            {
                Interact();
            }
        }*/


        public override void Interact()
        {
            if (pickUpBox != null)
            {
               pickUpBox.PutDown();
            }
            else
            {
                if (activeColor != Dimension.Color.NONE && player.transform.parent.transform.parent.gameObject.GetComponent<Dimension>().GetColor() != activeColor)
                    return;
                if(playerInZone)
                     PickUp();
            }
        }

        void PickUp()
        {
            pickUpBox = this;
            rb.isKinematic = true;
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            transform.SetParent(player);

            Vector3 relativeDis = transform.position - player.transform.position;
            transform.position = pos + pickUpOffset * (relativeDis / relativeDis.magnitude);
            transform.rotation = rot;
            Debug.Log("Picked Up");
        }

        public void PutDown()
        {
            Debug.Log("Put Down");
            rb.isKinematic = false;
            pickUpBox = null;
            transform.SetParent(player.parent.transform.parent);
            Vector3 localPos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.z));
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        public override void OnZoneEnter(Collider other) {
            if(other.gameObject.tag == "Player")
            {
                playerInZone = true;
                Debug.Log("Player Enter Box");
            }
        }
        public override void OnZoneStay(Collider other) {
            return;
        }

        public override void OnZoneExit(Collider other) {
            if(other.gameObject.tag == "Player" && pickUpBox != this)
            {
                playerInZone = false;
            }
        }

        public override bool IsInteracting() {
            return pickUpBox != null;
        }
    }
}