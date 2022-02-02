using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class PickableObject : Interactable
    {
        [SerializeField] Vector3 pickUpOffset = new Vector3(0.0f, 0.2f, 0.26f);
        Splittable.SplittableObject so;
        bool picked;

        void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
            picked = false;
        }

        public override void Interact()
        {
            if (!picked)
                PickUp();
            else
                PutDown();
        }

        void PickUp()
        {
            picked = true;
            transform.SetParent(InteractionManager.Instance.transform);
            transform.localPosition = pickUpOffset;
            transform.localRotation = Quaternion.identity;
        }

        public void PutDown()
        {
            picked = false;
            transform.SetParent(InteractionManager.Instance.transform.parent);
            Vector3 localPos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.RoundToInt(localPos.x), GetY(), Mathf.RoundToInt(localPos.z));
            transform.localRotation = Quaternion.identity;
            InteractionManager.Instance.ClearInteractable(this);
        }

        public override void OnZoneExit(Collider other)
        {
            if(!picked && other.gameObject.tag == "Player")
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
        }

        public override bool IsInteracting()
        {
            return picked;
        }

        float GetY()
        {
            RaycastHit raycastHit;
            bool hit = Physics.Raycast(transform.position, Vector3.down, out raycastHit);
            if (hit)
                return raycastHit.point.y;
            else
                return InteractionManager.Instance.transform.position.y;
        }
    }
}