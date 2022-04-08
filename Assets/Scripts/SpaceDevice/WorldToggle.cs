using UnityEngine;

namespace SpaceDevice
{
    [RequireComponent(typeof(Animator))]
    public class WorldToggle : MonoBehaviour
    {
        [SerializeField] SplitMergeMachine splitMergeMachine;
        Animator animator;
        bool pressed;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            pressed = false;
            Core.World.Instance.OnTransitionEnd.AddListener(UpdateMainToggle);
        }

        private void Start()
        {
            // Core.World.Instance.OnTransitionEnd.AddListener(UpdateMainToggle);
        }

        public void Toggle()
        {
            if (!EnergyBar.Instance.IsSufficient())
                return;
            if (SpaceDevice.Withdrawer.Instance != null && SpaceDevice.Withdrawer.Instance.IsOn)
                return;
            EnergyBar.Instance.CostSingleAction();
            animator.SetBool("IsOn", !Core.World.Instance.Splitted);
            pressed = true;
        }

        public void OnToggled()
        {
            if (pressed == true)
            {
                Core.World.Instance.Toggle();
                splitMergeMachine.Toggle();
                pressed = false;
            }
        }

        void UpdateMainToggle()
        {
            animator.SetBool("IsOn", Core.World.Instance.Splitted);
        }
    }
}