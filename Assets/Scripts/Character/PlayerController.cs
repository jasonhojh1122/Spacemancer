using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {

    [RequireComponent(typeof(Character.SplittablePlayer))]
    public class PlayerController : KinematicObject
    {
        public float maxSpeed = 5;
        public float jumpTakeOffSpeed = 3.5f;
        public float jumpDeceleration = 0.7f;
        public float rotateSpeed = 20.0f;
        public JumpState jumpState = JumpState.Grounded;
        public Collider playerCollider;
        public bool controlEnabled = true;

        [SerializeField] RuntimeAnimatorController idleAC;
        [SerializeField] RuntimeAnimatorController walkAC;
        [SerializeField] RuntimeAnimatorController jumpAC;

        bool jump;
        bool stopJump;
        Vector3 move;
        Animator animator;
        SplittablePlayer player;

        void Awake()
        {
            playerCollider = GetComponent<Collider>();
            animator = GetComponent<Animator>();
            player = GetComponent<SplittablePlayer>();
            animator.runtimeAnimatorController = idleAC;
        }

        protected override void Update()
        {
            if (paused)
            {
                animator.speed = 0.0f;
                player.SetDummyAnimatorSpeed(animator.speed);
                return;
            }

            if (controlEnabled&&!InputManager.Instance.pause)
            {
                animator.speed = 1.0f;
                player.SetDummyAnimatorSpeed(animator.speed);
                move.x = Input.GetAxis("Horizontal");
                move.z = Input.GetAxis("Vertical");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                    SetAnimatorController(jumpAC);
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                }

                if (move.x != 0 || move.z != 0) // change face direction when moveing
                {
                    Vector3 target = new Vector3(move.x, 0.0f, move.z);
                    transform.forward = Vector3.RotateTowards(transform.forward, target, rotateSpeed * Time.deltaTime, 0.0f);
                    if (jumpState == JumpState.Grounded)
                    {
                        SetAnimatorController(walkAC);
                    }
                }
                else if (jumpState == JumpState.Grounded)
                {
                    SetAnimatorController(idleAC);
                }
            }
            else
            {
                move.x = 0;
                move.z = 0;
            }
            UpdateJumpState();
            base.Update();
            player.UpdateDummyTransform();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    SetAnimatorController(idleAC);
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        void SetAnimatorController(RuntimeAnimatorController controller)
        {
            animator.runtimeAnimatorController = controller;
            player.SetDummyAnimatorController(controller);
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * jumpDeceleration;
                }
            }

            targetVelocity = move * maxSpeed;
        }
        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }

}