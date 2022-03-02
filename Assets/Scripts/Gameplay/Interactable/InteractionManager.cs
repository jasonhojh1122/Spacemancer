using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections;

using Input;

namespace Gameplay.Interactable
{
    [RequireComponent(typeof(IK.PlayerIK), typeof(Splittable.Character.Player))]
    public class InteractionManager : InputController
    {
        static InteractionManager _instance;
        public static InteractionManager Instance {
            get => _instance;
        }

        public bool IsInteracting {
            get {
                if (interactable == null) return false;
                else return interactable.IsInteracting();
            }
        }

        public IK.PlayerIK PlayerIK {
            get => playerIK;
        }

        IK.PlayerIK playerIK;
        Interactable interactable;
        InputAction interactAction;

        new void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instance of InteractionManager created");
            _instance = this;
            base.Awake();
            interactAction = playerInput.actions["Interaction"];
            playerIK = GetComponent<IK.PlayerIK>();
        }

        void Update()
        {
            if (IsPaused()) return;
            if (interactAction.triggered && interactable != null)
                interactable.Interact();
        }

        public void OnDimensionChange()
        {
            interactable = null;
        }

        public void SetInteractable(Interactable newInteractable)
        {
            if (interactable != null && interactable.IsInteracting())
                return;
            interactable = newInteractable;
        }

        public void ClearInteractable(Interactable oldInteractable)
        {
            if (interactable == null)
                return;
            else if (interactable.gameObject.GetInstanceID() == oldInteractable.gameObject.GetInstanceID())
                interactable = null;
        }
    }

}
