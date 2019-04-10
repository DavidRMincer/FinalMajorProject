using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    STANDING, CROUCHING, SNEAKING, WALKING, RUNNING, JUMPING,
    FALLING, VAULTING, SLIDING, MANTLING, CLIMBING
};

public enum StatePhase
{
    START, MIDDLE, END
};

public class Player_Script : MonoBehaviour
{

    private float               currentSpeed,
                                crouchHeight,
                                jumpHeight;
    private Rigidbody           rb;
    private MovementState       currentMoveState;
    private StatePhase          currentStatePhase;
    private bool                canSlide,
                                canVault,
                                canMantle,
                                canClimb;

    internal Vector3            startActionPoint,
                                middleActionPoint,
                                endActionPoint;
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
    }

    /////////////////////////////////////////////////////////////////
    // Updates gameplay and physics
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        switch (currentMoveState)
        {
            case MovementState.STANDING:
            case MovementState.CROUCHING:
            case MovementState.VAULTING:
            case MovementState.SLIDING:
            case MovementState.MANTLING:
                currentSpeed = 0.0f;
                break;

            case MovementState.WALKING:
                currentSpeed = walkSpeed;
                break;

            case MovementState.RUNNING:
                currentSpeed = runSpeed;
                break;
                
            case MovementState.SNEAKING:
                currentSpeed = sneakSpeed;
                break;

            case MovementState.CLIMBING:
                break;
            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Sets all action bools to false
    /////////////////////////////////////////////////////////////////
    internal void ResetActions()
    {
        canSlide = false;
        canVault = false;
        canMantle = false;
        canClimb = false;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if grounded
    /////////////////////////////////////////////////////////////////
    internal bool CanJump()
    {
        // Returns true if raycast collides with ground
        return Physics.Raycast(transform.position, -Vector3.up, (collider.height / 2) + 0.1f);
    }
    
    /////////////////////////////////////////////////////////////////
    // Moves gameobject
    /////////////////////////////////////////////////////////////////
    public void Move(float x, float z)
    {
        // Set current speed to walk speed
        float moveSpeed = currentSpeed * Time.deltaTime;

        // Calculate new rotation
        float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
        angle += Mathf.Atan2(   transform.position.x - camera.transform.position.x,
                                transform.position.z - camera.transform.position.z) * Mathf.Rad2Deg;

        // Apply rotation
        transform.rotation = Quaternion.Euler(Vector3.up * angle);

        // Calculate velocity
        Vector3 newVelocity = transform.forward * moveSpeed;
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
            if (CanVault())
            {
                // VAULT
            }

            else if (CanMantle())
            {
                // MANTLE
            }

            else if (CanClimb())
            {
                // CLIMB
            }

            // Apply upward jump force
            else rb.AddForce(Vector3.up * jumpForce);
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

    /////////////////////////////////////////////////////////////////
    // Returns can vault
    /////////////////////////////////////////////////////////////////
    public bool CanVault()
    {
        return canVault;
    }

    /////////////////////////////////////////////////////////////////
    // Returns can climb
    /////////////////////////////////////////////////////////////////
    public bool CanClimb()
    {
        return canClimb;
    }

    /////////////////////////////////////////////////////////////////
    // Returns can mantle
    /////////////////////////////////////////////////////////////////
    public bool CanMantle()
    {
        return canMantle;
    }

    /////////////////////////////////////////////////////////////////
    // Returns can slide
    /////////////////////////////////////////////////////////////////
    public bool CanSlide()
    {
        return canSlide;
    }

    /////////////////////////////////////////////////////////////////
    // Sets can slide
    /////////////////////////////////////////////////////////////////
    public void SetCanSlide(bool result)
    {
        canSlide = result;
    }

    /////////////////////////////////////////////////////////////////
    // Sets can vault
    /////////////////////////////////////////////////////////////////
    public void SetCanVault(bool result)
    {
        canVault = result;
    }

    /////////////////////////////////////////////////////////////////
    // Sets can mantle
    /////////////////////////////////////////////////////////////////
    public void SetCanMantle(bool result)
    {
        canMantle = result;
    }

    /////////////////////////////////////////////////////////////////
    // Sets can climb
    /////////////////////////////////////////////////////////////////
    public void SetCanClimb(bool result)
    {
        canClimb = result;
    }
}
