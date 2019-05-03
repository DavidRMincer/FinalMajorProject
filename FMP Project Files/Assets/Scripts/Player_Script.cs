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
                                standHeight,
                                crouchHeight,
                                jumpHeight,
                                vaultMidDuration,
                                mantleMidDuration,
                                currentActionTimer,
                                currentInputBlockTimer,
                                inputSpeed;
    private bool                canSlide,
                                canVault,
                                canMantle,
                                canClimb,
                                actionStarted,
                                inputBlocked;
    private Rigidbody           rb;
    private MovementState       currentMoveState;
    private StatePhase          currentStatePhase;
    private Vector3             actionOrigin;
    private Animation           playerAnimation;

    /////////////////////////////////////////////////////////////////

    internal Vector3            startActionPoint,
                                middleActionPoint,
                                endActionPoint;

    /////////////////////////////////////////////////////////////////

    public float                walkSpeed,
                                walkAnimSpeedMultiplier,
                                runSpeed,
                                runAnimSpeedMultiplier,
                                sneakSpeed,
                                sneakAnimSpeedMultiplier,
                                jumpForce,
                                jumpAnimSpeedMultiplier,
                                vaultDuration,
                                vaultSetupDuration,
                                vaultAnimSpeedMultiplier,
                                slideDuration,
                                slideSetupDuration,
                                slideAnimSpeedMultiplier,
                                mantleDuration,
                                mantleSetupDuration,
                                mantleAnimSpeedMultiplier,
                                climbDuration,
                                climbAnimSpeedMultiplier,
                                inputBlockTime,
                                minimumInputSpeed;
    public new Camera           camera;
    public GameObject           body;
    public new CapsuleCollider  collider;

    /////////////////////////////////////////////////////////////////
    // Runs on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        // Set objects
        rb = gameObject.GetComponent<Rigidbody>();
        playerAnimation = body.GetComponent<Animation>();


        // Set dimensions
        standHeight = collider.height;
        crouchHeight = standHeight / 2;
        jumpHeight = standHeight * 2;

        // Set action mid duration points
        vaultMidDuration = ((vaultDuration - vaultSetupDuration) / 2) + vaultSetupDuration;
        mantleMidDuration = ((mantleDuration - mantleSetupDuration) / 2) + mantleSetupDuration;

        // Set default values
        actionStarted = false;
        inputBlocked = false;
        currentMoveState = MovementState.STANDING;

        // Multiply animation speeds
        playerAnimation["Walking"].speed *= walkAnimSpeedMultiplier;
        playerAnimation["Running"].speed *= runAnimSpeedMultiplier;
        playerAnimation["Jumping"].speed *= jumpAnimSpeedMultiplier;
        playerAnimation["Walking"].speed *= runAnimSpeedMultiplier;
        playerAnimation["Sneaking"].speed *= sneakAnimSpeedMultiplier;
        playerAnimation["Vaulting"].speed *= vaultAnimSpeedMultiplier;
        playerAnimation["Sliding"].speed *= slideAnimSpeedMultiplier;
        playerAnimation["Mantling"].speed *= mantleAnimSpeedMultiplier;
        playerAnimation["Climbing"].speed *= climbAnimSpeedMultiplier;

        UpdateAnimation();
    }

    /////////////////////////////////////////////////////////////////
    // Updates gameplay and physics
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        // reset action started
        if (actionStarted)
            actionStarted = false;

        switch (currentMoveState)
        {
            case MovementState.VAULTING:
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
                // Set current speed
                currentSpeed = sneakSpeed;

                switch (currentStatePhase)
                {
                    case StatePhase.START:
                        if (currentActionTimer < climbDuration)
                        {
                            transform.position = Vector3.Lerp(actionOrigin, startActionPoint, currentActionTimer / climbDuration);
                            currentActionTimer += Time.deltaTime;
                        }
                        else
                        {
                            currentStatePhase = StatePhase.END;
                            currentActionTimer = 0.0f;
                        }
                        break;

                    default:
                        break;
                }
                break;

            case MovementState.JUMPING:
            case MovementState.FALLING:
                // Select action while jumping
                if (canClimb)
                    SetMovementState(MovementState.CLIMBING);
                else if (canMantle)
                    SetMovementState(MovementState.MANTLING);
                else if (canVault)
                    SetMovementState(MovementState.VAULTING);
                break;

            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Updates each frame
    /////////////////////////////////////////////////////////////////
    private void LateUpdate()
    {
        // Update animation speed
        if (currentMoveState == MovementState.WALKING)
            playerAnimation["Walking"].speed = walkAnimSpeedMultiplier * inputSpeed;
        else if (currentMoveState == MovementState.RUNNING)
            playerAnimation["Running"].speed = runAnimSpeedMultiplier * inputSpeed;
        else if (currentMoveState == MovementState.SNEAKING)
            playerAnimation["Sneaking"].speed = sneakAnimSpeedMultiplier * inputSpeed;
        else if (currentMoveState == MovementState.CLIMBING)
            playerAnimation["Climbing"].speed = climbAnimSpeedMultiplier * inputSpeed;
    }

    /////////////////////////////////////////////////////////////////
    // Updates animation
    /////////////////////////////////////////////////////////////////
    private void UpdateAnimation()
    {
        switch (currentMoveState)
        {
            case MovementState.STANDING:
                playerAnimation.Play("Standing");
                break;

            case MovementState.CROUCHING:
                playerAnimation.Play("Crouching");
                break;

            case MovementState.SNEAKING:
                playerAnimation.Play("Sneaking");
                break;

            case MovementState.WALKING:
                playerAnimation.Play("Walking");
                break;

            case MovementState.RUNNING:
                playerAnimation.Play("Running");
                break;

            case MovementState.JUMPING:
                playerAnimation.Play("Jumping");
                break;

            case MovementState.FALLING:
                playerAnimation.Play("Falling");
                break;

            case MovementState.VAULTING:
                playerAnimation.Play("Vaulting");
                break;

            case MovementState.SLIDING:
                playerAnimation.Play("Sliding");
                break;

            case MovementState.MANTLING:
                playerAnimation.Play("Mantling");
                break;
            
            default:
                playerAnimation.Play("Climbing");
                break;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Coroutine that blocks input for set time
    /////////////////////////////////////////////////////////////////
    private IEnumerator BlockInput()
    {
        // Block input
        inputBlocked = true;

        // Countdown
        for (currentInputBlockTimer = 0.0f; currentInputBlockTimer < inputBlockTime; currentInputBlockTimer += Time.deltaTime)
        {
            // Unblock once landed
            if (CanJump())
                currentInputBlockTimer = inputBlockTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Unblock input
        inputBlocked = false;
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
        // If input not blocked
        if (!inputBlocked)
        {
            Vector3 newVelocity = Vector3.zero;
            inputSpeed = new Vector2(x, z).magnitude;

            // Restrict input speed
            if (inputSpeed < minimumInputSpeed &&
                inputSpeed != 0.0f)
                inputSpeed = minimumInputSpeed;
            else if (inputSpeed > 1.0f)
                inputSpeed = 1.0f;

            // Set current speed to walk speed
            float moveSpeed = currentSpeed * inputSpeed * Time.deltaTime;

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
    }

    /////////////////////////////////////////////////////////////////
    // Applies upward force
    /////////////////////////////////////////////////////////////////
    public void Jump()
    {
        // If jump inputted and player grounded
        if (CanJump())
        {
            // Switch to available action
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
            else
                rb.AddForce(Vector3.up * jumpForce);
        }
        // Jump of wall if climbing
        else if (currentMoveState == MovementState.CLIMBING)
        {
            // Turn away from wall
            transform.Rotate(0.0f, 180.0f, 0.0f);
            // Apply jump force
            rb.AddForce((transform.forward + Vector3.up) * jumpForce);
            // Block input
            StartCoroutine("BlockInput");

            SetMovementState(MovementState.JUMPING);
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

        // Set gravity
        if (currentMoveState == MovementState.CLIMBING)
            GetComponent<Rigidbody>().useGravity = false;
        else
            GetComponent<Rigidbody>().useGravity = true;

        // Stop player from standing if can't stand
        if (currentMoveState == MovementState.STANDING &&
            !CanStand())
            currentMoveState = MovementState.CROUCHING;

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

        // Set collider height
        if (currentMoveState == MovementState.CROUCHING ||
            currentMoveState == MovementState.SNEAKING)
        {
            collider.height = crouchHeight;
            collider.transform.position = transform.position - (Vector3.up * crouchHeight * 0.51f);
        }
        else
        {
            collider.height = standHeight;
            collider.transform.position = transform.position;
        }

        // Update animation
        UpdateAnimation();
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
    // Returns stand height
    /////////////////////////////////////////////////////////////////
    public float GetStandHeight()
    {
        return standHeight;
    }

    /////////////////////////////////////////////////////////////////
    // Returns crouch height
    /////////////////////////////////////////////////////////////////
    public float GetCrouchHeight()
    {
        return crouchHeight;
    }

    /////////////////////////////////////////////////////////////////
    // Returns crouch height
    /////////////////////////////////////////////////////////////////
    public float GetJumpHeight()
    {
        return jumpHeight;
    }

    /////////////////////////////////////////////////////////////////
    // Returns width
    /////////////////////////////////////////////////////////////////
    public float GetWidth()
    {
        return collider.radius * 2;
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
    // Returns true if space to stand up
    /////////////////////////////////////////////////////////////////
    public bool CanStand()
    {
        float space = jumpHeight;
        RaycastHit[] ray = Physics.RaycastAll(  transform.position - (Vector3.up * collider.bounds.extents.y),
                                                Vector3.up * jumpHeight);

        for (int i = 0; i < ray.Length; ++i)
        {
            if (ray[i].collider.tag != "Player" &&
                ray[i].collider.tag != "Camera")
            {
                space = ray[i].distance;
            }
        }

        return space >= standHeight;
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
