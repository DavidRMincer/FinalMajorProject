using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Player_Script   playerScript;
    private float           currentXCam,
                            currentYCam;
    
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

        // If player inputs movement
        if (Input.GetAxis("Horizontal") != 0.0f ||
            Input.GetAxis("Vertical") != 0.0f)
        {
            // Pass input axis into move function
            playerScript.Move(  Input.GetAxis("Horizontal"),
                                Input.GetAxis("Vertical"));
        }

        // Jump if player inputs jump
        if (Input.GetButtonDown("Jump"))
            playerScript.Jump();
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
