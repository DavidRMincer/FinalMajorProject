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
        //Debug.Log(CanJump());
        Move();
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
    // Moves gameobject
    /////////////////////////////////////////////////////////////////
    public void Move()
    {
        // If player inputs movement
        if (Input.GetAxis("Horizontal") != 0.0f ||
            Input.GetAxis("Vertical") != 0.0f)
        {
            // Set current speed to walk speed
            currentSpeed = walkSpeed * Time.deltaTime;
            Debug.Log(currentSpeed);

            // Apply velocity
            Vector3 newVelocity = new Vector3(  Input.GetAxis("Horizontal") * currentSpeed,
                                                rb.velocity.y,
                                                Input.GetAxis("Vertical") * currentSpeed);
            rb.velocity = newVelocity;
            Debug.Log(rb.velocity);
        }
    }
}
