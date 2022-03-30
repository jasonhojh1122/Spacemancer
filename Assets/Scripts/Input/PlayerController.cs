
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : InputController
    {
        [SerializeField] float maxSpeed;
        [SerializeField] float rotateSpeed;
        [SerializeField] RuntimeAnimatorController idleAC;
        [SerializeField] RuntimeAnimatorController walkAC;
        [SerializeField] bool moving;
        [SerializeField] UnityEngine.Events.UnityEvent OnMovingStart;
        [SerializeField] UnityEngine.Events.UnityEvent OnMovingEnd;

        CharacterController characterController;
        InputAction moveAction;

        Animator animator;
        Splittable.Character.Player player;
        Vector3 vel;

        bool overrideControl;

        /// <summary>
        /// If the player's control is overridden by Timeline.
        /// </summary>
        public bool OverrideControl
        {
            get => overrideControl;
            set => overrideControl = value;
        }

        new void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            player = GetComponent<Splittable.Character.Player>();
            characterController = GetComponent<CharacterController>();
            moveAction = playerInput.actions["Movement"];
            animator.runtimeAnimatorController = idleAC;
            vel = Vector3.zero;
            moving = false;
            OverrideControl = false;
        }

        protected void Update()
        {
            if (OverrideControl)
            {
                return;
            }
            if (IsPaused())
            {
                vel = Vector3.zero;
            }
            else
            {
                UpdateVelocity();
            }
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
                    OnMovingStart.Invoke();
                }
            }
            else
            {
                if (moving)
                {
                    SetAnimatorController(idleAC);
                    moving = false;
                    OnMovingEnd.Invoke();
                }
            }
        }

        void SetAnimatorController(RuntimeAnimatorController controller)
        {
            animator.runtimeAnimatorController = controller;
            player.SetDummyAnimatorController(controller);
        }

        /// <summary>
        /// Sets the animation controller to walk.
        /// </summary>
        public void OverrideWalk()
        {
            animator.runtimeAnimatorController = walkAC;
            player.SetDummyAnimatorController(walkAC);
        }

        /// <summary>
        /// Sets the animation controller to IDLE.
        /// </summary>
        public void OverrideIdle()
        {
            animator.runtimeAnimatorController = idleAC;
            player.SetDummyAnimatorController(idleAC);
        }

    }

}