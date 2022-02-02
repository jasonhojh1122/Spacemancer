
using UnityEngine;
using UnityEngine.InputSystem;

namespace Splittable.Character {

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : InputController
    {
        [SerializeField] float maxSpeed;
        [SerializeField] float rotateSpeed;
        [SerializeField] RuntimeAnimatorController idleAC;
        [SerializeField] RuntimeAnimatorController walkAC;

        CharacterController characterController;
        InputAction moveAction;

        Animator animator;
        Player player;
        Vector3 vel;
        [SerializeField] bool moving;

        new void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            player = GetComponent<Player>();
            characterController = GetComponent<CharacterController>();
            moveAction = playerInput.actions["Movement"];
            animator.runtimeAnimatorController = idleAC;
            vel = Vector3.zero;
            moving = false;
        }

        private void Start() {

        }

        protected void Update()
        {
            if (pause) return;
            UpdateVelocity();
            UpdateMovingState();
            Move();
        }

        void Move()
        {
            characterController.SimpleMove(vel);
            transform.forward = Vector3.RotateTowards(transform.forward, vel, rotateSpeed * Time.deltaTime, 0.0f);
            player.UpdateDummyTransform();
        }

        void UpdateVelocity()
        {
            var dir2d = moveAction.ReadValue<Vector2>();
            vel = new Vector3(dir2d.x, 0, dir2d.y) * maxSpeed;
            if (Util.Fuzzy.CloseFloat(vel.magnitude, 0))
                vel = Vector3.zero;
        }

        void UpdateMovingState()
        {
            if (!Util.Fuzzy.CloseFloat(vel.magnitude, 0))
            {
                if (!moving)
                {
                    SetAnimatorController(walkAC);
                    moving = true;
                }
            }
            else
            {
                if (moving)
                {
                    SetAnimatorController(idleAC);
                    moving = false;
                }
            }
        }

        void SetAnimatorController(RuntimeAnimatorController controller)
        {
            animator.runtimeAnimatorController = controller;
            player.SetDummyAnimatorController(controller);
        }

    }

}