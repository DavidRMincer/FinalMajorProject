using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    public enum MovementState
    {
        STANDING, CROUCHING, SNEAKING, WALKING, RUNNING, JUMPING,
        FALLING, VAULTING, SLIDING, MANTLING, CLIMBING
    };

    public enum StatePhase
    {
        START, MIDDLE, END
    };

    private float               currentSpeed,
                                distToGround,
                                crouchHeight,
                                jumpHeight;
    private Rigidbody           rb;
    private MovementState       currentMoveState;
    private StatePhase          currentStatePhase;

    internal CapsuleCollider    collider;

    public float                walkSpeed,
                                runSpeed,
                                sneakSpeed,
                                jumpForce,
                                vaultDuration,
                                slideDuration,
                                mantleDuration;
    public Camera               camera;

    /////////////////////////////////////////////////////////////////
    // Runs on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        // Set objects
        rb = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<CapsuleCollider>();

        // Set dimensions
        crouchHeight = collider.height / 2;
        jumpHeight = collider.height * 2;
        distToGround = crouchHeight + 0.1f;
    }

    /////////////////////////////////////////////////////////////////
    // Updates gameplay and physics
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if grounded
    /////////////////////////////////////////////////////////////////
    private bool CanJump()
    {
        // Returns true if raycast collides with ground
        return Physics.Raycast(transform.position, -Vector3.up, distToGround);
    }
    
    /////////////////////////////////////////////////////////////////
    // Moves gameobject
    /////////////////////////////////////////////////////////////////
    public void Move(float x, float z)
    {
        // Set current speed to walk speed
        currentSpeed = walkSpeed * Time.fixedDeltaTime;

        // Calculate new rotation
        float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
        angle += Mathf.Atan2(   transform.position.x - camera.transform.position.x,
                                transform.position.z - camera.transform.position.z) * Mathf.Rad2Deg;

        // Apply rotation
        transform.rotation = Quaternion.Euler(Vector3.up * angle);

        // Calculate velocity
        Vector3 newVelocity = transform.forward * currentSpeed;
        newVelocity.y = rb.velocity.y;

        // Apply velocity
        rb.velocity = newVelocity;
    }

    /////////////////////////////////////////////////////////////////
    // Applies upward force
    /////////////////////////////////////////////////////////////////
    public void Jump()
    {
        // If jump inputted and player grounded
        if (CanJump())
        {
            // Apply upward jump force
            rb.AddForce(Vector3.up * jumpForce);
        }
    }

    /////////////////////////////////////////////////////////////////
    // Sets movement state
    /////////////////////////////////////////////////////////////////
    public void SetMovementState(MovementState state)
    {
        // Set movement state as state
        currentMoveState = state;
        // Set current state to start
        currentStatePhase = StatePhase.START;
    }

    /////////////////////////////////////////////////////////////////
    // Returns current move state
    /////////////////////////////////////////////////////////////////
    public MovementState GetMovementState()
    {
        return currentMoveState;
    }

    /////////////////////////////////////////////////////////////////
    // Returns current state phase
    /////////////////////////////////////////////////////////////////
    public StatePhase GetStatePhase()
    {
        return currentStatePhase;
    }
}
