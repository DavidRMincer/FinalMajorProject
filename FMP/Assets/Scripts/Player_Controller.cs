using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Player_Script   playerScript;
    private float           currentXCam,
                            currentYCam;

    /////////////////////////////////////////////////////////////////

    public float            cameraDistance,
                            cameraSpeed,
                            minimumCameraAngle,
                            maximumCameraAngle;

    /////////////////////////////////////////////////////////////////
    // Runs on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Set player script
        playerScript = gameObject.GetComponent<Player_Script>();
    }

    /////////////////////////////////////////////////////////////////
    // Updates gameplay and physics
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        PlayerInput();
    }

    /////////////////////////////////////////////////////////////////
    // Updates each frame
    /////////////////////////////////////////////////////////////////
    public void LateUpdate()
    {
        // Update camera
        UpdateCamera();
    }

    /////////////////////////////////////////////////////////////////
    // Takes player input
    /////////////////////////////////////////////////////////////////
    public void PlayerInput()
    {
        // CAMERA

        if (Input.GetAxis("Mouse X") != 0.0f ||
            Input.GetAxis("Mouse Y") != 0.0f)
        {
            MoveCamera( Input.GetAxis("Mouse X"),
                        Input.GetAxis("Mouse Y"));
        }

        // MOVEMENT

        switch (playerScript.GetMovementState())
        {
            case MovementState.STANDING:

                // Player falls
                if (!playerScript.CanJump())
                    playerScript.SetMovementState(MovementState.FALLING);

                // Player crouchs
                else if (Input.GetButtonDown("Crouch"))
                    playerScript.SetMovementState(MovementState.CROUCHING);

                // If player inputs movement
                else if (   Input.GetAxis("Horizontal") != 0.0f ||
                            Input.GetAxis("Vertical") != 0.0f)
                    playerScript.SetMovementState(MovementState.WALKING);

                // Player jumps
                else if (Input.GetButtonDown("Jump"))
                    playerScript.Jump();
                break;

            case MovementState.CROUCHING:

                // Player falls
                if (!playerScript.CanJump())
                    playerScript.SetMovementState(MovementState.FALLING);

                // Player stands up
                else if (Input.GetButtonDown("Crouch"))
                    playerScript.SetMovementState(MovementState.STANDING);

                // If player inputs movement
                else if (   Input.GetAxis("Horizontal") != 0.0f ||
                            Input.GetAxis("Vertical") != 0.0f)
                    playerScript.SetMovementState(MovementState.SNEAKING);
                break;

            case MovementState.SNEAKING:

                // Player falls
                if (!playerScript.CanJump())
                    playerScript.SetMovementState(MovementState.FALLING);

                // Player stands up
                else if (Input.GetButtonDown("Crouch"))
                    playerScript.SetMovementState(MovementState.WALKING);

                // Player stops moving
                else if (   Input.GetAxis("Horizontal") == 0.0f &&
                            Input.GetAxis("Vertical") == 0.0f)
                    playerScript.SetMovementState(MovementState.CROUCHING);

                // Player moves
                else playerScript.Move( Input.GetAxis("Horizontal"),
                                        Input.GetAxis("Vertical"));
                break;

            case MovementState.WALKING:

                // Player falls
                if (!playerScript.CanJump())
                    playerScript.SetMovementState(MovementState.FALLING);

                // Player crouchs
                else if (Input.GetButtonDown("Crouch"))
                    playerScript.SetMovementState(MovementState.SNEAKING);

                // Player jumps
                else if (Input.GetButtonDown("Jump"))
                    playerScript.Jump();

                // Player runs
                else if (Input.GetButton("Sprint"))
                    playerScript.SetMovementState(MovementState.RUNNING);

                // Player stops moving
                else if (   Input.GetAxis("Horizontal") == 0.0f &&
                            Input.GetAxis("Vertical") == 0.0f)
                    playerScript.SetMovementState(MovementState.STANDING);

                // Player moves
                else playerScript.Move( Input.GetAxis("Horizontal"),
                                        Input.GetAxis("Vertical"));
                break;

            case MovementState.RUNNING:

                // Player falls
                if (!playerScript.CanJump())
                    playerScript.SetMovementState(MovementState.FALLING);

                // Player slides
                else if (Input.GetButtonDown("Crouch") &&
                            playerScript.CanSlide())
                    playerScript.SetMovementState(MovementState.SLIDING);

                // Player jumps
                else if (Input.GetButtonDown("Jump"))
                    playerScript.Jump();

                // Player runs
                else if (!Input.GetButton("Sprint"))
                    playerScript.SetMovementState(MovementState.WALKING);

                // Player stops moving
                else if (   Input.GetAxis("Horizontal") == 0.0f &&
                            Input.GetAxis("Vertical") == 0.0f)
                    playerScript.SetMovementState(MovementState.STANDING);

                // Player moves
                else playerScript.Move( Input.GetAxis("Horizontal"),
                                        Input.GetAxis("Vertical"));
                break;

            case MovementState.JUMPING:
            case MovementState.FALLING:

                // Player lands
                if (playerScript.CanJump())
                {
                    playerScript.SetMovementState(MovementState.STANDING);
                }

                // If player inputs movement
                if (Input.GetAxis("Horizontal") != 0.0f ||
                    Input.GetAxis("Vertical") != 0.0f)
                {
                    // Pass input axis into move function
                    playerScript.Move(Input.GetAxis("Horizontal"),
                                        Input.GetAxis("Vertical"));
                }
                break;

            case MovementState.CLIMBING:
                if (playerScript.GetStatePhase() == StatePhase.END)
                {
                    // Player on ground
                    if (playerScript.CanJump())
                        playerScript.SetMovementState(MovementState.STANDING);

                    // Player jumps
                    else if (   Input.GetButtonDown("Jump"))
                                playerScript.Jump();

                    // Player moves
                    else playerScript.Move( Input.GetAxis("Horizontal"),
                                            Input.GetAxis("Vertical"));
                }
                break;

            default:
                break;
        }

        // FOR TESTING PURPOSES ONLY!

        if (Input.GetKeyDown("1"))
        {
            playerScript.SetMovementState(MovementState.VAULTING);
        }

        if (Input.GetKeyDown("2"))
        {
            playerScript.SetMovementState(MovementState.SLIDING);
        }

        if (Input.GetKeyDown("3"))
        {
            playerScript.SetMovementState(MovementState.MANTLING);
        }

        if (Input.GetKeyDown("4"))
        {
            playerScript.SetMovementState(MovementState.CLIMBING);
        }
    }

    /////////////////////////////////////////////////////////////////
    // Moves camera
    /////////////////////////////////////////////////////////////////
    public void MoveCamera(float x, float y)
    {
        // Move camera
        currentXCam += (x * cameraSpeed * Time.deltaTime);
        currentYCam += -(y * cameraSpeed * Time.deltaTime);

        // Keep y angle with minimum and maximum ranges
        if (currentYCam < maximumCameraAngle)
            currentYCam = maximumCameraAngle;
        else if (currentYCam > minimumCameraAngle)
            currentYCam = minimumCameraAngle;
    }

    /////////////////////////////////////////////////////////////////
    // Updates camera rotation
    /////////////////////////////////////////////////////////////////
    public void UpdateCamera()
    {
        // Calculate camera position
        Vector3 distVec = new Vector3(0, 0, cameraDistance);
        Quaternion camRotation = Quaternion.Euler(currentYCam, currentXCam, 0);

        // Apply new camera position
        playerScript.camera.transform.position = gameObject.transform.position - camRotation * distVec;

        // Rotate towards player
        playerScript.camera.transform.LookAt(gameObject.transform.position);

        // OBSTRUCTION DETECTION

        // Calculate ray direction
        Vector3 direction = Camera.main.transform.position - transform.position;

        // Raycast from player to camera
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction);

        // If ray hits an obstruction
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.tag != "MainCamera" &&
                hits[i].collider.tag != "Player")
            {
                // Move camera to point of obstruction
                playerScript.camera.transform.position = hits[i].point;
                i = hits.Length;
            }
        }
    }
}
