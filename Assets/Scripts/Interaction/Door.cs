using UnityEngine;

namespace Interaction
{
    public class Door : Interactable
    {
        [SerializeField] string sceneName;

        public override void Interact()
        {

        }

        public override bool IsInteracting()
        {
            return false;
        }
    }
}
