
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Splittable.ErrorSpace))]
    public class ErrorSpaceFixer : Interactable
    {
        Splittable.ErrorSpace errorSpace;
        Collider col;

        new protected void Awake()
        {
            base.Awake();
            errorSpace = GetComponent<Splittable.ErrorSpace>();
        }

        public override void Interact()
        {
            errorSpace.Fix();
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }

}