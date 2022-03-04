
using UnityEngine;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Splittable.ErrorSpace))]
    public class ErrorSpaceFixer : Interactable
    {
        static SpaceDevice.SpaceFragmentContainer container;
        Splittable.ErrorSpace errorSpace;
        Collider col;

        void Awake()
        {
            errorSpace = GetComponent<Splittable.ErrorSpace>();
            if (container == null)
                container = FindObjectOfType<SpaceDevice.SpaceFragmentContainer>();
        }

        public override void Interact()
        {
            if (!container.IsSufficient()) return;
            errorSpace.Fix();
            container.Lose();
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }

}