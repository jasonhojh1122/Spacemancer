using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Invector.vCharacterController;

namespace Interaction {
    [RequireComponent(typeof(Core.SplittableObject))]
    public class Box : Interactable
    {
        // [SerializeField] Dimension.Color activeColor; // None if no restriction
        [SerializeField] float pickUpOffset = 0.05f;
        // public static Box pickUpBox = null;
        static PlayerInteraction player = null;
        Rigidbody rb;
        Core.SplittableObject so;
        bool playerInZone;
        bool picked;

        void Awake()
        {
            if (player == null)
                player = FindObjectOfType<PlayerInteraction>();
            so = GetComponent<Core.SplittableObject>();
            rb = GetComponent<Rigidbody>();
            picked = false;
        }

        public override void Interact()
        {
            if (!picked)
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
            // pickUpBox = this;
            Destroy(rb);
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            transform.SetParent(player.transform);

            Vector3 relativeDis = transform.position - player.transform.position;
            transform.position = pos + pickUpOffset * (relativeDis / relativeDis.magnitude);
            transform.rotation = rot;
            picked = true;
        }

        public void PutDown()
        {
            AddRigidBody();
            picked = false;
            transform.SetParent(player.transform.parent);
            Vector3 localPos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.z));
            transform.localRotation = Quaternion.identity;
            player.ClearInteractable(this);
        }

        void AddRigidBody()
        {
            rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
  
        }

        public override void OnZoneEnter(Collider other) {
            if(other.gameObject.tag == "Player")
            {
                player.SetInteractable(this);
            }
        }

        public override void OnZoneStay(Collider other) {
            return;
        }

        public override void OnZoneExit(Collider other) {
            if(!picked && other.gameObject.tag == "Player")
            {
                player.ClearInteractable(this);
            }
        }

        public override bool IsInteracting() {
            return picked;
        }
    }
}