using UnityEngine;

namespace SpaceDevice
{
    [RequireComponent(typeof(Animator))]
    public class WithdrawerToggle : MonoBehaviour
    {
        [SerializeField] Withdrawer withdrawer;
        Animator animator;
        bool pressed;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            pressed = false;
        }

        private void Start()
        {
            withdrawer.OnToggle.AddListener(UpdateToggle);
        }

        public void Toggle()
        {
            animator.SetBool("IsOn", !withdrawer.IsOn);
            pressed = true;
        }

        public void OnToggled()
        {
            if (pressed == true)
            {
                withdrawer.Toggle();
                pressed = false;
            }
        }

        void UpdateToggle()
        {
            animator.SetBool("IsOn", withdrawer.IsOn);
        }
    }
}