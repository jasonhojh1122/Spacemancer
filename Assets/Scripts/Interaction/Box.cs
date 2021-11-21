using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Invector.vCharacterController;

namespace Interaction {
    [RequireComponent(typeof(Core.SplittableObject))]
    public class Box : Interactable
    {
        // [SerializeField] Dimension.Color activeColor; // None if no restriction
        [SerializeField] float pickUpOffset = 0.1f;
        public static Box pickUpBox = null;
        static Character.SplittablePlayer player = null;
        Rigidbody rb;
        Core.SplittableObject so;
        bool playerInZone;

        void Awake()
        {
            if (player == null)
                player = FindObjectOfType<Character.SplittablePlayer>();
            so = GetComponent<Core.SplittableObject>();
            rb = GetComponent<Rigidbody>();
        }

        public override void Interact()
        {
            if (pickUpBox != null)
            {
               pickUpBox.PutDown();
            }
            else
            {
                if (!so.IsInCorrectDim())
                    return;
                if(playerInZone)
                    PickUp();
            }
        }

        void PickUp()
        {
            pickUpBox = this;
            Destroy(rb);
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            transform.SetParent(player.transform);

            Vector3 relativeDis = transform.position - player.transform.position;
            transform.position = pos + pickUpOffset * (relativeDis / relativeDis.magnitude);
            transform.rotation = rot;
            Debug.Log("Picked Up");
        }

        public void PutDown()
        {
            rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            Debug.Log("Put Down");
            pickUpBox = null;
            transform.SetParent(player.transform.parent);
            Vector3 localPos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y), Mathf.RoundToInt(localPos.z));
            transform.localRotation = Quaternion.identity;
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