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
        // Disable cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        // EXIT
        if (Input.GetButtonDown("Exit"))
            Application.Quit();

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
                else if (   Input.GetButton("Sprint") ||
                            Input.GetAxis("Sprint") == 1)
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
                else if (Input.GetButton("Crouch") &&
                            playerScript.CanSlide())
                    playerScript.SetMovementState(MovementState.SLIDING);

                // Player jumps
                else if (Input.GetButtonDown("Jump"))
                    playerScript.Jump();

                // Player runs
                else if (   !Input.GetButton("Sprint") &&
                            Input.GetAxis("Sprint") != 1)
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
                    else if (Input.GetButtonDown("Jump"))
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
        currentYCam += (y * cameraSpeed * Time.deltaTime);

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
        RaycastHit[]    topRay,
                        midRay,
                        botRay;
        bool obstructed = false;
        Vector3 distVec = new Vector3(0, 0, cameraDistance);
        Quaternion camRotation = Quaternion.Euler(currentYCam, currentXCam, 0);

        // Apply new camera position
        playerScript.camera.transform.position = gameObject.transform.position - camRotation * distVec;

        // Rotate towards player
        playerScript.camera.transform.LookAt(gameObject.transform.position);

        // OBSTRUCTION DETECTION

        // Does not check during action
        if (playerScript.GetMovementState() != MovementState.VAULTING &&
            playerScript.GetMovementState() != MovementState.SLIDING &&
            playerScript.GetMovementState() != MovementState.MANTLING)
        {
            // From stomach

            // Raycast from player to camera
            midRay = Physics.RaycastAll(transform.position, (Camera.main.transform.position - transform.position).normalized);

            // If ray hits an obstruction
            for (int i = 0; i < midRay.Length; i++)
            {
                if (midRay[i].collider.tag != "MainCamera" &&
                    midRay[i].collider.tag != "Player" &&
                    midRay[i].distance < cameraDistance)
                {
                    // OBSTRUCTED
                    obstructed = true;
                    i = midRay.Length;
                }
            }

            // From feet
            if (obstructed)
            {
                // Reset obstaucted bool
                obstructed = false;

                botRay = Physics.RaycastAll(transform.position + (Vector3.up * playerScript.collider.bounds.extents.y),
                    Camera.main.transform.position - (transform.position + (Vector3.up * playerScript.collider.bounds.extents.y)));

                // If ray hits an obstruction
                for (int i = 0; i < botRay.Length; i++)
                {
                    if (botRay[i].collider.tag != "MainCamera" &&
                        botRay[i].collider.tag != "Player" &&
                        botRay[i].distance < cameraDistance)
                    {
                        // OBSTRUCTED
                        obstructed = true;
                        i = midRay.Length;
                    }
                }
            }

            // From head
            if (obstructed)
            {
                topRay = Physics.RaycastAll(transform.position + (Vector3.up * playerScript.collider.bounds.extents.y),
                    Camera.main.transform.position - (transform.position + (Vector3.up * playerScript.collider.bounds.extents.y)));

                // If ray hits an obstruction
                for (int i = 0; i < topRay.Length; i++)
                {
                    if (topRay[i].collider.tag != "MainCamera" &&
                        topRay[i].collider.tag != "Player" &&
                        topRay[i].distance < cameraDistance)
                    {
                        // Move camera to point of obstruction
                        playerScript.camera.transform.position = topRay[i].point;
                        i = midRay.Length;
                    }
                }
            }
        }
    }
}
