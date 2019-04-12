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
                                jumpHeight,
                                vaultMidDuration,
                                mantleMidDuration,
                                currentActionTimer;
    private bool                canSlide,
                                canVault,
                                canMantle,
                                canClimb,
                                actionStarted;
    private Rigidbody           rb;
    private MovementState       currentMoveState;
    private StatePhase          currentStatePhase;
    private Vector3             actionOrigin;

    /////////////////////////////////////////////////////////////////

    internal Vector3            startActionPoint,
                                middleActionPoint,
                                endActionPoint;
    internal CapsuleCollider    collider;

    /////////////////////////////////////////////////////////////////

    public float                walkSpeed,
                                runSpeed,
                                sneakSpeed,
                                jumpForce,
                                vaultDuration,
                                vaultSetupDuration,
                                slideDuration,
                                slideSetupDuration,
                                mantleDuration,
                                mantleSetupDuration,
                                climbDuration,
                                climbSetupDuration;
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

        // Set action mid duration points
        vaultMidDuration = ((vaultDuration - vaultSetupDuration) / 2) + vaultSetupDuration;
        mantleMidDuration = ((mantleDuration - mantleSetupDuration) / 2) + mantleSetupDuration;

        // Set default values
        actionStarted = false;
    }

    /////////////////////////////////////////////////////////////////
    // Updates gameplay and physics
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        Debug.Log(currentActionTimer + " " + currentStatePhase);
        // reset action started
        if (actionStarted)
            actionStarted = false;

        switch (currentMoveState)
        {
            case MovementState.VAULTING:
                Debug.Log(currentStatePhase);
                switch (currentStatePhase)
                {
                    case StatePhase.START:
                        if (currentActionTimer < vaultSetupDuration)
                        {
                            transform.position = Vector3.Lerp(actionOrigin, startActionPoint, currentActionTimer / vaultSetupDuration);
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.MIDDLE;
                        }
                        break;

                    case StatePhase.MIDDLE:
                        if (currentActionTimer < vaultMidDuration)
                        {
                            transform.position = Vector3.Lerp(startActionPoint, middleActionPoint, (currentActionTimer - vaultSetupDuration) / (vaultMidDuration - vaultSetupDuration));
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.END;
                        }
                        break;

                    default:
                        if (currentActionTimer < vaultDuration)
                        {
                            transform.position = Vector3.Lerp(middleActionPoint, endActionPoint, (currentActionTimer - vaultMidDuration) / (vaultDuration - vaultMidDuration));
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            SetMovementState(MovementState.STANDING);
                            currentActionTimer = 0.0f;
                        }
                        break;
                }
                break;

            case MovementState.SLIDING:
                switch (currentStatePhase)
                {
                    case StatePhase.START:
                        if (currentActionTimer < slideSetupDuration)
                        {
                            transform.position = Vector3.Lerp(actionOrigin, startActionPoint, currentActionTimer / slideSetupDuration);
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.END;
                        }
                        break;
                    
                    default:
                        if (currentActionTimer < slideDuration)
                        {
                            transform.position = Vector3.Lerp(startActionPoint, endActionPoint, (currentActionTimer - slideSetupDuration) / (slideDuration - slideSetupDuration));
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            SetMovementState(MovementState.STANDING);
                            currentActionTimer = 0.0f;
                        }
                        break;
                }
                break;

            case MovementState.MANTLING:
                switch (currentStatePhase)
                {
                    case StatePhase.START:
                        if (currentActionTimer < mantleSetupDuration)
                        {
                            transform.position = Vector3.Lerp(actionOrigin, startActionPoint, currentActionTimer / mantleSetupDuration);
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.MIDDLE;
                        }
                        break;

                    case StatePhase.MIDDLE:
                        if (currentActionTimer < mantleMidDuration)
                        {
                            transform.position = Vector3.Lerp(startActionPoint, middleActionPoint, (currentActionTimer - mantleSetupDuration) / (mantleMidDuration - mantleSetupDuration));
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.END;
                        }
                        break;

                    default:
                        if (currentActionTimer < mantleDuration)
                        {
                            transform.position = Vector3.Lerp(middleActionPoint, endActionPoint, (currentActionTimer - mantleMidDuration) / (mantleDuration - mantleMidDuration));
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            SetMovementState(MovementState.STANDING);
                            currentActionTimer = 0.0f;
                        }
                        break;
                }
                break;

            case MovementState.WALKING:
                // Set current speed
                currentSpeed = walkSpeed;
                break;

            case MovementState.RUNNING:
                // Set current speed
                currentSpeed = runSpeed;
                break;
                
            case MovementState.SNEAKING:
                // Set current speed
                currentSpeed = sneakSpeed;
                break;

            case MovementState.CLIMBING:
                switch (currentStatePhase)
                {
                    case StatePhase.START:
                        if (currentActionTimer < climbSetupDuration)
                        {
                            transform.position = Vector3.Lerp(actionOrigin, startActionPoint, currentActionTimer / climbSetupDuration);
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.MIDDLE;
                        }
                        break;

                    case StatePhase.MIDDLE:
                        if (currentActionTimer < climbDuration)
                        {
                            transform.position = Vector3.Lerp(startActionPoint, endActionPoint, (currentActionTimer - climbSetupDuration) / (climbDuration - climbSetupDuration));
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.END;
                            collider.enabled = true;
                            currentActionTimer = 0.0f;
                        }
                        break;

                    default:
                        // Set current speed
                        currentSpeed = sneakSpeed;
                        break;
                }
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
    // Returns action started
    /////////////////////////////////////////////////////////////////
    internal bool ActionStarted()
    {
        return actionStarted;
    }

    /////////////////////////////////////////////////////////////////
    // Moves gameobject
    /////////////////////////////////////////////////////////////////
    public void Move(float x, float z)
    {
        Vector3 newVelocity = Vector3.zero;

        // Set current speed to walk speed
        float moveSpeed = currentSpeed * Time.deltaTime;

        // ON FOOT
        if (currentMoveState != MovementState.CLIMBING)
        {
            // Calculate new rotation
            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
            angle += Mathf.Atan2(transform.position.x - camera.transform.position.x,
                                    transform.position.z - camera.transform.position.z) * Mathf.Rad2Deg;

            // Apply rotation
            transform.rotation = Quaternion.Euler(Vector3.up * angle);

            // Calculate velocity
            newVelocity = transform.forward * moveSpeed;
            newVelocity.y = rb.velocity.y;
        }
        // CLIMBING
        else
        {
            // Calculate velocity
            newVelocity += transform.right * (x * moveSpeed);
            newVelocity += transform.up * (z * moveSpeed);
        }

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
                SetMovementState(MovementState.VAULTING);
            }

            else if (CanMantle())
            {
                SetMovementState(MovementState.MANTLING);
            }

            else if (CanClimb())
            {
                SetMovementState(MovementState.CLIMBING);
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

        if (currentMoveState == MovementState.VAULTING ||
            currentMoveState == MovementState.SLIDING ||
            currentMoveState == MovementState.MANTLING ||
            (currentMoveState == MovementState.CLIMBING && currentStatePhase == StatePhase.START))
        {
            // Setup for actions
            actionStarted = true;
            actionOrigin = transform.position;
            collider.enabled = false;
        }
        else collider.enabled = true;
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
