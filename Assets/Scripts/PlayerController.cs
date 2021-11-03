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
    public float maxSpeed = 7;
    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
    public float jumpTakeOffSpeed = 7;
    public float jumpDeceleration = 0.5f;
    public JumpState jumpState = JumpState.Grounded;
    private bool stopJump;
    /*internal new*/
    public Collider playerCollider;
    /*internal new*/
    //public Health health;
    public bool controlEnabled = true;

    bool jump;
    Vector3 move;
    SpriteRenderer spriteRenderer;

    //internal Animator animator;

    //public Bounds Bounds => collider2d.bounds;
    void Awake()
    {
        //health = GetComponent<Health>();
        playerCollider = GetComponent<Collider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
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
                jumpState = JumpState.PrepareToJump;
            else if (Input.GetButtonUp("Jump"))
            {
                stopJump = true;
            }
            // change face direction when moveing
            if (move.x != 0 || move.y != 0)
            {
                //transform.rotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z));
                transform.forward = new Vector3(move.x, 0, move.z);
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