using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    private float       currentSpeed,
                        distToGround;
    private Rigidbody   rb;
    private Collider    collider;

    public float        walkSpeed,
                        jumpForce;

    /////////////////////////////////////////////////////////////////
    // Sets private variables on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
        distToGround = collider.bounds.extents.y + 0.1f;
    }

    private void FixedUpdate()
    {
        
        Jump();
        Debug.Log(rb.velocity.x / Time.fixedDeltaTime);
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if grounded
    /////////////////////////////////////////////////////////////////
    public bool CanJump()
    {
        // Returns true if raycast collides with ground
        return Physics.Raycast(transform.position, -Vector3.up, distToGround);
    }

    /////////////////////////////////////////////////////////////////
    // Takes player input
    /////////////////////////////////////////////////////////////////
    public void PlayerInput()
    {
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
    // Moves gameobject
    /////////////////////////////////////////////////////////////////
    public void Move(   float x,
                        float y)
    {
        // Set current speed to walk speed
        currentSpeed = walkSpeed * Time.fixedDeltaTime;

        // Calculate velocity
        Vector3 newVelocity = new Vector3(Input.GetAxis("Horizontal") * currentSpeed,
                                            rb.velocity.y,
                                            Input.GetAxis("Vertical") * currentSpeed);
        // Apply velocity
        rb.velocity = newVelocity;
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
