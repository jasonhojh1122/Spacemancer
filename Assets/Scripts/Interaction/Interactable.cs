using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] IK.IKSetting ikSetting;

        protected Splittable.SplittableObject so;

        public IK.IKSetting IKSetting {
            get => ikSetting;
        }

        protected void Awake()
        {
            so = GetComponent<Splittable.SplittableObject>();
        }

        public abstract void Interact();
        public abstract bool IsInteracting();

        public virtual void OnZoneEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                InteractionManager.Instance.SetInteractable(this);
            }
        }

        public virtual void OnZoneStay(Collider other)
        {
            return;
        }

        public virtual void OnZoneExit(Collider other) {
            if (other.gameObject.tag == "Player")
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
        }
    }

}