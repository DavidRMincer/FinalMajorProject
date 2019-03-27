using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    private float               currentSpeed,
                                distToGround,
                                currentXCam,
                                currentYCam;
    private Rigidbody           rb;

    internal Collider           collider;

    public float                walkSpeed,
                                jumpForce,
                                cameraDistance,
                                cameraSpeed,
                                minimumCameraAngle,
                                maximumCameraAngle;
    public Camera               camera;
    public Obstacle_Detector    obstacleDetector;

    /////////////////////////////////////////////////////////////////
    // Sets private variables on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
        distToGround = collider.bounds.extents.y + 0.1f;

        obstacleDetector.SetDimensions(GetComponent<Player_Script>());

        Cursor.lockState = CursorLockMode.Locked;
    }

    /////////////////////////////////////////////////////////////////
    // Updates more than FPS
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        PlayerInput();
        Jump();
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
    // Takes player input
    /////////////////////////////////////////////////////////////////
    public void PlayerInput()
    {
        // CAMERA

        if (    Input.GetAxis("Mouse X") != 0.0f ||
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
            Move(Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical"));
        }
        // Current speed is 0 when no movement is inputted
        else currentSpeed = 0.0f;
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
    // Moves gameobject
    /////////////////////////////////////////////////////////////////
    public void Move(   float x,
                        float z)
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
    // Moves camera
    /////////////////////////////////////////////////////////////////
    public void MoveCamera(     float x,
                                float y)
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
        Vector3 distVec = new Vector3(0, 0, cameraDistance);
        Quaternion camRotation = Quaternion.Euler(currentYCam, currentXCam, 0);

        // Apply new camera position
        camera.transform.position = gameObject.transform.position - camRotation * distVec;

        // Rotate towards player
        camera.transform.LookAt(gameObject.transform.position);
        
        // OBSTRUCTION DETECTION

        RaycastHit hit;
        // Calculate ray direction
        Vector3 direction = Camera.main.transform.position - transform.position;

        // If ray hits an obstruction
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.collider.tag != "MainCamera")
            {
                // Move camera to point of obstruction
                camera.transform.position = hit.point;
            }
        }
    }

    /////////////////////////////////////////////////////////////////
    // Applies upward force
    /////////////////////////////////////////////////////////////////
    public void Jump()
    {
        // If jump inputted and player grounded
        if (Input.GetButtonDown("Jump") &&
            CanJump())
        {
            // Apply upward jump force
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
}
