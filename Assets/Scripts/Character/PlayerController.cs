
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character {

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float maxSpeed;
        [SerializeField] float rotateSpeed;
        [SerializeField] RuntimeAnimatorController idleAC;
        [SerializeField] RuntimeAnimatorController walkAC;

        CharacterController characterController;
        PlayerInput playerInput;
        InputAction moveAction;

        Animator animator;
        SplittablePlayer player;
        Vector3 vel;
        [SerializeField] bool moving;
        [SerializeField] bool onLattice;
        [SerializeField] bool onGround;

        void Awake()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<SplittablePlayer>();
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
            moveAction = playerInput.actions["Movement"];
            animator.runtimeAnimatorController = idleAC;
            vel = Vector3.zero;
            moving = false;
            onLattice = true;
        }

        protected void Update()
        {
            UpdateVelocity();
            UpdateMovingState();
            Move();
        }

        void Move() {
            onGround = characterController.SimpleMove(vel);
            transform.forward = Vector3.RotateTowards(transform.forward, vel, rotateSpeed * Time.deltaTime, 0.0f);
        }

        void UpdateVelocity() {
            var dir2d = moveAction.ReadValue<Vector2>();
            vel = new Vector3(dir2d.x, 0, dir2d.y) * maxSpeed;
            if (Util.Fuzzy.CloseFloat(vel.magnitude, 0)) {
                vel = Vector3.zero;
            }
        }

        void UpdateMovingState() {
            if (!Util.Fuzzy.CloseFloat(vel.magnitude, 0)) {
                if (!moving) {
                    SetAnimatorController(walkAC);
                    moving = true;
                }
            }
            else {
                if (moving) {
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