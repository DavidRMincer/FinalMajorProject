using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Detector : MonoBehaviour
{
    private float       standingHeight,
                        crouchingHeight,
                        jumpHeight,
                        playerWidth;
    private RaycastHit  topHit,
                        midHit,
                        botHit,
                        upperHit;
    private GameObject  obstacle;

    public float        detectorLength;
    public Color        debugColour;

    private void FixedUpdate()
    {
        // DETECT OBSTACLE IN FRONT

        Vector3 top = transform.position + (Vector3.up * standingHeight / 2),
                bot = transform.position - (Vector3.up * standingHeight / 2),
                upper = top + (transform.forward * detectorLength) + (Vector3.up * standingHeight);

        // Raycast forwards from mid body
        Physics.Raycast(transform.position, transform.forward, out midHit);
        Debug.DrawRay(transform.position, transform.forward * detectorLength, debugColour, 0.01f);

        // Raycast forwards from head
        Physics.Raycast(top, transform.forward, out topHit);
        Debug.DrawRay(top, transform.forward * detectorLength, debugColour, 0.01f);

        // Raycast forwards from feet
        Physics.Raycast(bot, transform.forward, out botHit);
        Debug.DrawRay(bot, transform.forward * detectorLength, debugColour, 0.01f);

        // Raycast forwards and up from head
        Physics.Raycast(top, upper - top, out upperHit);
        Debug.DrawRay(top, upper - top, debugColour, 0.01f);

        bool hit = false;

        if      (topHit.collider != null && topHit.distance <= detectorLength && topHit.distance != 0.0f)
        {
            obstacle = topHit.collider.gameObject;
            hit = true;
        }
        else if (botHit.collider != null && botHit.distance <= detectorLength && botHit.distance != 0.0f)
        {
            obstacle = botHit.collider.gameObject;
            hit = true;
        }
        else if (midHit.collider != null && midHit.distance <= detectorLength && midHit.distance != 0.0f)
        {
            obstacle = midHit.collider.gameObject;
            hit = true;
        }
        else if (upperHit.collider != null && upperHit.distance <= detectorLength && upperHit.distance != 0.0f)
        {
            obstacle = upperHit.collider.gameObject;
            hit = true;
        }

        if (hit)
        {
            //DO STUFF
            Debug.Log(CanSlide());
        }
    }

    /////////////////////////////////////////////////////////////////
    // Sets dimensions from player dimensions
    /////////////////////////////////////////////////////////////////
    public void SetDimensions(Player_Script player)
    {
        // Set standing height as player height
        standingHeight = player.collider.bounds.size.y;
        // Set crouching height as half player height
        crouchingHeight = standingHeight / 2;
        // Set player width as player width
        playerWidth = player.collider.bounds.size.z;
        // Set jump height
        jumpHeight = standingHeight * 2;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if player can vault obstacle
    /////////////////////////////////////////////////////////////////
    public bool CanVault()
    {
        // DO STUFF
        return false;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if player can climb obstacle
    /////////////////////////////////////////////////////////////////
    public bool CanClimb()
    {
        // DO STUFF
        return false;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if player can mantle obstacle
    /////////////////////////////////////////////////////////////////
    public bool CanMantle()
    {
        // DO STUFF
        return false;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if player can slide under obstacle
    /////////////////////////////////////////////////////////////////
    public bool CanSlide()
    {
        Vector3 obstacleBot = obstacle.transform.position;
        RaycastHit slideRay;

        obstacleBot.y = obstacle.transform.position.y - obstacle.GetComponent<Collider>().bounds.extents.y;

        // Raycast down from bottom of obstacle to ground
        Physics.Raycast(obstacleBot, -Vector3.up, out slideRay);
        Debug.DrawLine(obstacleBot, obstacleBot - (Vector3.up * detectorLength), debugColour, 0.01f);

        // If distance from ground is more than crouching height
        if (slideRay.distance != 0.0f &&
            slideRay.distance >= crouchingHeight)
        {
            return true;
        }

        // DO STUFF
        return false;
    }
}
