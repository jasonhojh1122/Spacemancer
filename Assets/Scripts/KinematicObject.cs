using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements game physics for some in game entity.
/// </summary>
public class KinematicObject : MonoBehaviour
{
    /// <summary>
    /// The minimum normal (dot product) considered suitable for the entity sit on.
    /// </summary>
    [SerializeField] protected float minGroundNormalY = .65f;

    /// <summary>
    /// A custom gravity coefficient applied to this entity.
    /// </summary>
    [SerializeField] protected float gravityModifier = 1f;

    /// <summary>
    /// The current velocity of the entity.
    /// </summary>
    [SerializeField] protected Vector3 velocity;

    /// <summary>
    /// Is the entity currently sitting on a surface?
    /// </summary>
    public bool IsGrounded { get; private set; }
    public bool paused;

    protected Vector3 targetVelocity;
    protected Vector3 groundNormal;
    protected Rigidbody body;

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;



    /// <summary>
    /// Bounce the object's vertical velocity.
    /// </summary>
    /// <param name="value"></param>
    public void Bounce(float value)
    {
        velocity.y = value;
    }

    /// <summary>
    /// Bounce the objects velocity in a direction.
    /// </summary>
    /// <param name="dir"></param>
    public void Bounce(Vector3 dir)
    {
        velocity.z = dir.z;
        velocity.y = dir.y;
        velocity.x = dir.x;
    }

    /// <summary>
    /// Teleport to some position.
    /// </summary>
    /// <param name="position"></param>
    public void Teleport(Vector3 position)
    {
        body.position = position;
        velocity *= 0;
        body.velocity *= 0;
    }

    protected virtual void OnEnable()
    {
        body = GetComponent<Rigidbody>();
        //body.isKinematic = true;
    }

    protected virtual void OnDisable()
    {
        //body.isKinematic = false;
    }

    protected virtual void Start()
    {
        groundNormal = Vector3.up;
    }

    protected virtual void Update()
    {
        if (paused)
        {
            return;
        }
        targetVelocity = Vector3.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        //if already falling, fall faster than the jump speed, otherwise use normal gravity.

        if (velocity.y < 0)
            velocity += gravityModifier * Physics.gravity * Time.deltaTime;
        else
            velocity += Physics.gravity * Time.deltaTime;

        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;

        IsGrounded = false;

        var deltaPosition = velocity * Time.deltaTime;

        var moveXZ = new Vector3(deltaPosition.x, 0,deltaPosition.z);
        var move = Vector3.ProjectOnPlane(moveXZ, groundNormal);
        PerformMovement(move, false);

        move = Vector3.up * deltaPosition.y;

        PerformMovement(move, true);

    }

    void PerformMovement(Vector3 move, bool yMovement)
    {

        var distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            //check if we hit anything in current direction of travel
            var hitBuffer = body.SweepTestAll(move, distance + shellRadius);
            for (var i = 0; i < hitBuffer.Length; i++)
            {
                if (hitBuffer[i].collider.isTrigger) continue;

                var currentNormal = hitBuffer[i].normal;

                //is this surface flat enough to land on?
                if (currentNormal.y > minGroundNormalY)
                {
                    IsGrounded = true;
                    // if moving up, change the groundNormal to new surface normal.
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                        currentNormal.z = 0;
                    }
                }
                if (IsGrounded)
                {
                    //how much of our velocity aligns with surface normal?
                    var projection = Vector3.Dot(velocity, currentNormal);
                    if (projection < 0)
                    {
                        //slower velocity if moving against the normal (up a hill).
                        velocity = velocity - projection * currentNormal;
                    }
                }
                else
                {
                    //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                    velocity.x *= 0;
                    velocity.z *= 0;
                    velocity.y = Mathf.Min(velocity.y, 0);
                }
                //remove shellDistance from actual move distance.
                var modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        body.position = body.position + move.normalized * distance;
    }

}