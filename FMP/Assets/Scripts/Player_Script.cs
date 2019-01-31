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
    CLIMBING,
    SLIDING
};

public class Player_Script : MonoBehaviour {

    private float           currentSpeed;
    private movementState   currentMoveState;
    private Rigidbody       rb;
    private Vector3         moveDirection;
    private bool            canJump;

    public float            walkSpeed,
                            runSpeed,
                            jumpSpeed,
                            slideSpeed,
                            climbSpeed,
                            currentJumpForce,
                            sneakSpeed;

	// Use this for initialization
	void Start ()
    {
        canJump = false;
        currentMoveState = movementState.STATIONARY;
        rb = GetComponent<Rigidbody>();
	}

    private void FixedUpdate()
    {
        playerInput();
        playerMove(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Verticle"));
    }

    // Change state based on player input
    void playerInput()
    {
        switch (currentMoveState)
        {
            case movementState.STATIONARY:
                //Change to jump
                if (Input.GetButtonDown("Jump"))
                {
                    currentMoveState = movementState.JUMPING;
                }
                //Change to crouch
                else if (Input.GetButtonDown("Crouch"))
                {
                    currentMoveState = movementState.CROUCHING;
                }
                //Change to walking
                else if (Input.GetAxis("Vertical") != 0 ||
                    Input.GetAxis("Horizontal") != 0)
                {
                    currentMoveState = movementState.WALKING;
                }
                break;

            case movementState.CROUCHING:
                //Change to stationary
                if (!Input.GetButtonDown("Crouch"))
                {
                    currentMoveState = movementState.STATIONARY;
                }
                //Change to sneaking
                else if (Input.GetAxis("Vertical") != 0 ||
                    Input.GetAxis("Horizontal") != 0)
                {
                    currentMoveState = movementState.SNEAKING;
                }
                break;

            case movementState.WALKING:
                //Change to jump
                if (Input.GetButtonDown("Jump"))
                {
                    currentMoveState = movementState.JUMPING;
                }
                //Change to sprinting
                else if (Input.GetButtonDown("Sprint"))
                {
                    currentMoveState = movementState.RUNNING;
                }
                //Change to sneaking
                else if (Input.GetButtonDown("Crouch"))
                {
                    currentMoveState = movementState.SNEAKING;
                }
                break;

            case movementState.SNEAKING:
                //Change to walking
                if (!Input.GetButtonDown("Crouch"))
                {
                    currentMoveState = movementState.WALKING;
                }
                break;

            case movementState.RUNNING:
                //Change to walking
                if (!Input.GetButtonDown("Sprint"))
                {
                    currentMoveState = movementState.WALKING;
                }
                //Change to jumping
                else if (Input.GetButtonDown("Jump"))
                {
                    currentMoveState = movementState.JUMPING;
                }
                //Change to sliding
                else if (Input.GetButtonDown("Crouch"))
                {
                    currentMoveState = movementState.SLIDING;
                }
                break;

            case movementState.JUMPING:
                break;

            case movementState.CLIMBING:
                //Change to jumping
                if (Input.GetButtonDown("Jump"))
                {
                    currentMoveState = movementState.JUMPING;
                }
                break;

            case movementState.SLIDING:
                break;

            default:
                break;
        }
    }

    // Move player
    void playerMove(float horizontal, float vertical)
    {
        switch (currentMoveState)
        {
            case movementState.WALKING:
                moveDirection.z = (vertical * walkSpeed * Time.deltaTime);
                moveDirection.x = (horizontal * walkSpeed * Time.deltaTime);
                break;

            case movementState.SNEAKING:
                moveDirection.z = (vertical * sneakSpeed * Time.deltaTime);
                moveDirection.x = (horizontal * sneakSpeed * Time.deltaTime);
                break;

            case movementState.RUNNING:
                moveDirection.z = (vertical * runSpeed * Time.deltaTime);
                moveDirection.x = (horizontal * runSpeed * Time.deltaTime);
                break;

            case movementState.JUMPING:
                break;

            case movementState.CLIMBING:
                moveDirection.y = (vertical * climbSpeed * Time.deltaTime);
                moveDirection.x = (horizontal * climbSpeed * Time.deltaTime);
                break;

            case movementState.SLIDING:
                break;

            default:
                break;
        }
    }
}
