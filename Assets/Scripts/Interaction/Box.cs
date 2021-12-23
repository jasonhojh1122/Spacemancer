using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Invector.vCharacterController;

namespace Interaction
{
    [RequireComponent(typeof(Core.SplittableObject))]
    public class Box : Interactable
    {
        [SerializeField] float pickUpOffset = 0.05f;
        Rigidbody rb;
        Core.SplittableObject so;
        bool playerInZone;
        public bool picked;

        protected new void Awake()
        {
            base.Awake();
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
            Destroy(rb);
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            transform.SetParent(player.transform);

            Vector3 relativeDis = transform.position - player.transform.position;
            transform.position = pos + pickUpOffset * (relativeDis / relativeDis.magnitude) + new Vector3(0,0.5f,0);
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