
using UnityEngine;

namespace Splittable.Character
{

    [RequireComponent(typeof(Animator))]
    public class PlayerDummy : MonoBehaviour
    {

        Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetAnimatorController(RuntimeAnimatorController controller)
        {
            animator.runtimeAnimatorController = controller;
        }

        public void SetAnimatorSpeed(float speed)
        {
            animator.speed = speed;
        }

    }

}