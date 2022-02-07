
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Splittable.ErrorSpace))]
    public class ErrorSpaceFixer : Interactable
    {
        static SpaceDevice.SpaceFragmentContainer container;
        Splittable.ErrorSpace errorSpace;
        Collider col;

        new protected void Awake()
        {
            base.Awake();
            errorSpace = GetComponent<Splittable.ErrorSpace>();
            if (container == null)
                container = FindObjectOfType<SpaceDevice.SpaceFragmentContainer>();
        }

        public override void Interact()
        {
            if (container.Count <= 0) return;
            errorSpace.Fix();
            container.Count -= 1;
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }

}