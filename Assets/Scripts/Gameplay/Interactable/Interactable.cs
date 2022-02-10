using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Interactable
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] IK.IKSetting ikSetting;

        public IK.IKSetting IKSetting {
            get => ikSetting;
        }

        /// <summary>
        /// <c>Interact</c> is called when the player presses the interaction button.
        /// One should check if the player is in the correct dimension.
        /// </summary>
        public abstract void Interact();

        /// <summary>
        /// Returns if the interaction is ongoing, such as playing animations, holding
        /// objects, etc.
        /// </summary>
        /// <returns> True if the interaction is doing something, otherwise false. </returns>
        public abstract bool IsInteracting();

        /// <summary>
        /// <c>OnZoneEnter</c> is called when a gameobject enters the interaction zone.
        /// By default, <c>Interactable</c> registers the interaction to the <c>InteractionManager</c>.
        /// </summary>
        /// <param name="other"> The <c>Collider</c> of the gameobject. </param>
        public virtual void OnZoneEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                InteractionManager.Instance.SetInteractable(this);
            }
        }

        /// <summary>
        /// <c>OnZoneStay</c> is called when a gameobject stays in the interaction zone.
        /// By default, <c>Interactable</c> does nothing.
        /// </summary>
        /// <param name="other"> The <c>Collider</c> of the gameobject. </param>
        public virtual void OnZoneStay(Collider other)
        {
            return;
        }

        /// <summary>
        /// <c>OnZoneExit</c> is called when a gameobject exits the interaction zone.
        /// By default, <c>Interactable</c> unregisters the interaction to the <c>InteractionManager</c> .
        /// </summary>
        /// <param name="other"> The <c>Collider</c> of the gameobject. </param>
        public virtual void OnZoneExit(Collider other) {
            if (other.gameObject.tag == "Player")
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
        }
    }

}