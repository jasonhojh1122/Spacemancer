using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Collections;

namespace Interaction
{
    [RequireComponent(typeof(IK.PlayerIK))]
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

        IK.PlayerIK playerIK;
        Interactable interactable;
        InputAction interactAction;
        Splittable.SplittableObject interactableSo;
        Collider interactableCol;

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
            if (pause) return;
            if (interactAction.triggered && interactable != null)
                StartCoroutine(Interact());
            if (IsInteracting && interactable.IKSetting != null)
                playerIK.SetIKTarget(interactable.IKSetting);
        }

        IEnumerator Interact()
        {
            interactable.BeforePose();
            if (interactable.IKSetting != null)
                yield return StartCoroutine(playerIK.Pose(interactable.IKSetting));
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
            if (interactable != null)
            {
                interactableCol = interactable.GetComponent<Collider>();
                interactableSo = interactable.GetComponent<Splittable.SplittableObject>();
            }
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
