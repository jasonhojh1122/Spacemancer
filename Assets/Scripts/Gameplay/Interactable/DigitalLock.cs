using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(Core.ObjectColor))]
    public class DigitalLock : Interactable
    {
        [SerializeField] UnityEvent OnUnlock;

        Core.ObjectColor objectColor;

        private void Awake()
        {
            objectColor = GetComponent<Core.ObjectColor>();
        }

        public override void Interact()
        {
            if (objectColor.Color == Core.World.Instance.ActiveDimension.color)
                OnUnlock.Invoke();
        }

        public override bool IsInteracting()
        {
            return false;
        }
    }
}
