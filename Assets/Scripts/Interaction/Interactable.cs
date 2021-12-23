using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
        public abstract class Interactable : MonoBehaviour
        {

            protected static PlayerInteraction player = null;

            protected void Awake()
            {
                if (player == null)
                    player = FindObjectOfType<PlayerInteraction>();
            }

            public abstract void Interact();
            public abstract bool IsInteracting();

            public virtual void OnZoneEnter(Collider other)
            {
                if (other.gameObject.tag == "Player")
                {
                    player.SetInteractable(this);
                }
            }

            public virtual void OnZoneStay(Collider other)
            {
                return;
            }

            public virtual void OnZoneExit(Collider other) {
                if (other.gameObject.tag == "Player")
                {
                    player.ClearInteractable(this);
                }
            }
        }

}