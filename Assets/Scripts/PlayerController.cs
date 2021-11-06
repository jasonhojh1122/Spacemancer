using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the main class used to implement control of the player.
/// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
/// </summary>
public class PlayerController : KinematicObject
{

    /// <summary>
    /// Max horizontal speed of the player.
    /// </summary>
    public float maxSpeed = 5;
    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
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
    SpriteRenderer spriteRenderer;
    Animator animator;

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = idleAC;
    }

    protected override void Update()
    {
        if (paused)
        {
            return;
        }
        if (controlEnabled)
        {
            move.x = Input.GetAxis("Horizontal");
            move.z = Input.GetAxis("Vertical");
            if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
            {
                jumpState = JumpState.PrepareToJump;
                animator.runtimeAnimatorController = jumpAC;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                stopJump = true;
            }
            if (move.x != 0 || move.z != 0) // change face direction when moveing
            {
                //transform.localRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
                //transform.rotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
                // transform.forward = new Vector3(move.x, 0, move.z);
                Vector3 target = new Vector3(move.x, 0.0f, move.z);
                transform.forward = Vector3.RotateTowards(transform.forward, target, rotateSpeed * Time.deltaTime, 0.0f);
                if (jumpState == JumpState.Grounded) {
                    animator.runtimeAnimatorController = walkAC;
                }
            }
            else if (jumpState == JumpState.Grounded){
                animator.runtimeAnimatorController = idleAC;
            }
        }
        else
        {
            move.x = 0;
            move.z = 0;
        }
        UpdateJumpState();
        base.Update();
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
                    //Schedule<PlayerJumped>().player = this;
                    jumpState = JumpState.InFlight;
                }
                break;
            case JumpState.InFlight:
                if (IsGrounded)
                {
                    //Schedule<PlayerLanded>().player = this;
                    jumpState = JumpState.Landed;
                }
                break;
            case JumpState.Landed:
                animator.runtimeAnimatorController = idleAC;
                jumpState = JumpState.Grounded;
                break;
        }
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

        /*if (move.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (move.x < -0.01f)
            spriteRenderer.flipX = true;*/

        //animator.SetBool("grounded", IsGrounded);
        //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

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
