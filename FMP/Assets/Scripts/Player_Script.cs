using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum movementState
{
    STATIONARY,
    CROUCHING,
    WALKING,
    SNEAKING,
    RUNNING,
    JUMPING,
    FALLING,
    VAULTING,
    CLIMBING,
    MANTLING,
    SLIDING
};

public class Player_Script : MonoBehaviour {

    private float           currentSpeed,
                            currentVaultDuration,
                            currentMantleDuration,
                            currentSlideDuration,
                            currentJumpSpeed,
                            distFromYBoundary,
                            height;
    private movementState   currentMoveState;
    private Rigidbody       rb;
    private Vector3         moveDirection;
    private bool            canClimb,
                            canVault,
                            canMantle;
    private Collider        playerCollider;

    public Camera           playerCamera;
    public float            walkSpeed,
                            runSpeed,
                            jumpSpeed,
                            slideSpeed,
                            climbSpeed,
                            gravity,
                            terminalVelocity,
                            sneakSpeed,
                            vaultDuration,
                            mantleDuration,
                            slideDuration;

	
	void Start ()
    {
        //Set action conditions to false
        canClimb = false;
        canVault = false;
        canMantle = false;

        //Set current action durations to max
        currentVaultDuration = vaultDuration;
        currentMantleDuration = mantleDuration;
        currentSlideDuration = slideDuration;

        //Set default movement state
        currentMoveState = movementState.STATIONARY;

        //Set rigidbody
        rb = GetComponent<Rigidbody>();

        //Set collider
        playerCollider = GetComponent<Collider>();

        //Set height
        height = playerCollider.bounds.size.y;

        //Set distance from ground
        distFromYBoundary = playerCollider.bounds.extents.y;

    }

    private void FixedUpdate()
    {
        UpdateMoveState();
        SetRotation();
        Move();
        Debug.Log(currentMoveState);
    }

    ////////////////////////////////////////////
    //Update current movement state
    ////////////////////////////////////////////
    public void UpdateMoveState()
    {
        switch (currentMoveState)
        {
            case movementState.STATIONARY:
                //Change to falling
                if (!OnGround())
                    Fall();

                //Change to walking
                else if (Input.GetAxis("Vertical") != 0.0f ||
                    Input.GetAxis("Horizontal") != 0.0f)
                    Walk();

                //Change to crouching
                else if (Input.GetButtonDown("Crouch"))
                    Crouch();

                //Change to jumping
                else if (Input.GetButtonDown("Jump"))
                    Jump();
                break;

            case movementState.CROUCHING:
                //Change to falling
                if (!OnGround())
                    Fall();

                //Change to stationary
                else if (Input.GetButtonDown("Crouch"))
                    Stationary();

                //Change to sneaking
                else if (Input.GetAxis("Vertical") != 0.0f ||
                    Input.GetAxis("Horizontal") != 0.0f)
                    Sneak();
                break;

            case movementState.WALKING:
                //Change to falling
                if (!OnGround())
                    Fall();

                //Change to stationary
                else if (Input.GetAxis("Vertical") == 0.0f &&
                    Input.GetAxis("Horizontal") == 0.0f)
                    Stationary();

                //Change to running
                else if (Input.GetButtonDown("Sprint"))
                    Run();
                break;

            case movementState.SNEAKING:
                //Change to falling
                if (!OnGround())
                    Fall();

                //Change to stationary
                else if (Input.GetAxis("Vertical") == 0.0f &&
                    Input.GetAxis("Horizontal") == 0.0f)
                    Crouch();

                //Change to running
                else if (Input.GetButtonDown("Crouch"))
                    Walk();
                break;

            case movementState.RUNNING:
                //Change to falling
                if (!OnGround())
                    Fall();

                //Change to walking
                else if (!Input.GetButton("Sprint"))
                    Walk();

                //Change to stationary
                else if (Input.GetAxis("Vertical") == 0.0f &&
                    Input.GetAxis("Horizontal") == 0.0f)
                    Stationary();

                //Change to jumping
                else if (Input.GetButtonDown("Jump"))
                    Jump();

                //Change to sliding
                else if (Input.GetButtonDown("Crouch"))
                    Slide();
                break;

            case movementState.JUMPING:
                //Change to stationary
                if (OnGround())
                    Stationary();

                //Change to falling
                if (rb.velocity.y < 0.0f)
                    Fall();
                break;

            case movementState.FALLING:
                //Change to stationary
                if (OnGround())
                    Stationary();
                break;

            case movementState.VAULTING:
                //Set to stationary after vaulting obsticle
                if (currentVaultDuration <= 0.0f)
                    Stationary();

                //Still vaulting obsticle
                else
                    //Countdown vault duration
                    currentVaultDuration -= Time.deltaTime;
                break;

            case movementState.CLIMBING:
                //Change to falling
                if (!canClimb)
                    Fall();

                else if (canMantle &&
                    Input.GetAxis("Vertical") > 0.0f)
                    Mantle();
                break;

            case movementState.MANTLING:
                //Set to stationary after mantling obsticle
                if (currentMantleDuration <= 0.0f)
                    Stationary();

                //Countdown mantle duration
                else
                    currentMantleDuration -= Time.deltaTime;
                break;

            case movementState.SLIDING:
                //Set to stationary after sliding
                if (currentSlideDuration <= 0.0f)
                    Stationary();

                //Still sliding
                else
                    //Countdown slide duration
                    currentSlideDuration -= Time.deltaTime;
                break;

            default:
                break;
        }
    }

    ////////////////////////////////////////////
    //Update current movement state
    ////////////////////////////////////////////
    public void Move()
    {
        if (currentMoveState == movementState.WALKING ||
            currentMoveState == movementState.RUNNING ||
            currentMoveState == movementState.SNEAKING ||
            currentMoveState == movementState.JUMPING ||
            currentMoveState == movementState.FALLING)
        {
            Vector3 velocity = (rb.transform.forward * currentSpeed) * Time.fixedDeltaTime;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
        }
    }

    ////////////////////////////////////////////
    //Set to stationary
    ////////////////////////////////////////////
    private void SetRotation()
    {
        Debug.Log("SetRotation()");
        //Set angle as player rotation
        float angle = rb.transform.rotation.y;

        Debug.Log(angle);

        //While player is moving
        if (currentMoveState == movementState.WALKING ||
            currentMoveState == movementState.RUNNING ||
            currentMoveState == movementState.SNEAKING ||
            currentMoveState == movementState.JUMPING ||
            currentMoveState == movementState.FALLING)
        {
            //Set rotation angle
            angle = Mathf.Atan2(Input.GetAxis("Vertical"),
                    Input.GetAxis("Horizontal")) * Mathf.Rad2Deg;
            Debug.Log(angle);
            angle += playerCamera.transform.rotation.y;
        }

        Debug.Log(angle);

        rb.transform.rotation = new Quaternion(rb.transform.rotation.x,
            angle,
            rb.transform.rotation.z,
            rb.transform.rotation.w);
    }

    ////////////////////////////////////////////
    //Set to stationary
    ////////////////////////////////////////////
    private void Stationary()
    {
        //Set to stationary if can stand
        if (CanStand())
        {
            currentSpeed = walkSpeed;
            currentMoveState = movementState.STATIONARY;
        }

        //Crouch if cannot stand
        else
        {
            currentSpeed = sneakSpeed;
            Crouch();
        }
    }

    ////////////////////////////////////////////
    //Set to crouching
    ////////////////////////////////////////////
    private void Crouch()
    {
        currentSpeed = sneakSpeed;
        currentMoveState = movementState.CROUCHING;
    }

    ////////////////////////////////////////////
    //Set to walking
    ////////////////////////////////////////////
    private void Walk()
    {
        currentSpeed = walkSpeed;
        currentMoveState = movementState.WALKING;
    }

    ////////////////////////////////////////////
    //Set to sneaking
    ////////////////////////////////////////////
    private void Sneak()
    {
        currentSpeed = sneakSpeed;
        currentMoveState = movementState.SNEAKING;
    }

    ////////////////////////////////////////////
    //Set to running
    ////////////////////////////////////////////
    private void Run()
    {
        currentSpeed = runSpeed;
        currentMoveState = movementState.RUNNING;
    }

    ////////////////////////////////////////////
    //Set to jumping
    ////////////////////////////////////////////
    private void Jump()
    {
        rb.AddForce(0.0f, jumpSpeed, 0.0f);
        currentMoveState = movementState.JUMPING;
    }

    ////////////////////////////////////////////
    //Set to falling
    ////////////////////////////////////////////
    private void Fall()
    {
        currentMoveState = movementState.FALLING;
    }

    ////////////////////////////////////////////
    //Set to vaulting
    ////////////////////////////////////////////
    private void Vault()
    {
        //Reset slide counter
        currentVaultDuration = vaultDuration;

        currentMoveState = movementState.VAULTING;
    }

    ////////////////////////////////////////////
    //Set to climbing
    ////////////////////////////////////////////
    private void Climb()
    {
        currentSpeed = climbSpeed;
        currentMoveState = movementState.CLIMBING;
    }

    ////////////////////////////////////////////
    //Set to mantling
    ////////////////////////////////////////////
    private void Mantle()
    {
        //Reset mantle counter
        currentMantleDuration = mantleDuration;

        currentMoveState = movementState.MANTLING;
    }

    ////////////////////////////////////////////
    //Set to sliding
    ////////////////////////////////////////////
    private void Slide()
    {
        currentSpeed = slideSpeed;

        //Reset slide counter
        currentSlideDuration = slideDuration;

        currentMoveState = movementState.SLIDING;
    }

    ////////////////////////////////////////////
    //Returns true if colliding with ground
    ////////////////////////////////////////////
    private bool OnGround()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distFromYBoundary + 0.1f);
    }

    ////////////////////////////////////////////
    //Returns true if space to stand up
    ////////////////////////////////////////////
    private bool CanStand()
    {
        return !Physics.Raycast(transform.position, Vector3.up, height - distFromYBoundary);
    }
}
