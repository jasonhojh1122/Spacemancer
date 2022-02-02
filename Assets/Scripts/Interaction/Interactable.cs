using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
        public abstract class Interactable : MonoBehaviour
        {
            [SerializeField] IK.IKSetting ikSetting;

            public IK.IKSetting IKSetting {
                get => ikSetting;
            }

            public abstract void Interact();
            public abstract bool IsInteracting();
            public virtual void BeforePose()
            {
                return;
            }

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