using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Detector : MonoBehaviour
{
    private enum        rayHit
    {
        NONE, BOT, MID, TOP, UPPER
    };

    private float       standingHeight,
                        crouchingHeight,
                        jumpHeight,
                        playerWidth;
    private RaycastHit  topHit,
                        midHit,
                        botHit,
                        upperHit;
    private GameObject  obstacle;
    private rayHit      currentRayHit;

    public float        detectorLength;
    public Color        debugColour,
                        slideDebugColour,
                        vaultDebugColour,
                        mantleDebugColour,
                        climbDebugColour;

    private void FixedUpdate()
    {
        //// DETECT OBSTACLE IN FRONT

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
        
        //// ON IMPACT

        if (HitObstacle())
        {
            // Checks if player can slide underneath
            CanSlide();
            // Checks if player can vault over
            Debug.Log(CanVault());
            // Checks if player can mantle onto
            CanMantle();
            // Checks if player can climb up
            CanClimb();
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
    // Returns true if a ray hits an obstacle
    /////////////////////////////////////////////////////////////////
    private bool HitObstacle()
    {
        currentRayHit = rayHit.NONE;

        // Middle raycast hits
        if (midHit.collider != null && midHit.distance <= detectorLength && midHit.distance != 0.0f)
        {
            obstacle = midHit.collider.gameObject;
            currentRayHit = rayHit.MID;
            return true;
        }
        // Bottom raycast hits
        else if (botHit.collider != null && botHit.distance <= detectorLength && botHit.distance != 0.0f)
        {
            obstacle = botHit.collider.gameObject;
            currentRayHit = rayHit.BOT;
            return true;
        }
        // Top raycast hits
        else if (topHit.collider != null && topHit.distance <= detectorLength && topHit.distance != 0.0f)
        {
            obstacle = topHit.collider.gameObject;
            currentRayHit = rayHit.TOP;
            return true;
        }
        // Upper raycast hits
        else if (upperHit.collider != null && upperHit.distance <= detectorLength && upperHit.distance != 0.0f)
        {
            obstacle = upperHit.collider.gameObject;
            currentRayHit = rayHit.UPPER;
            return true;
        }

        return false;
    }

    private Vector3 GetHitPoint()
    {
        // return hit point from the raycast that hit obstacle
        switch (currentRayHit)
        {
            case rayHit.BOT:
                return botHit.point;
            case rayHit.MID:
                return midHit.point;
            case rayHit.TOP:
                return topHit.point;
            case rayHit.UPPER:
                return upperHit.point;
            default:
                return obstacle.transform.position;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if player can vault obstacle
    /////////////////////////////////////////////////////////////////
    public bool CanVault()
    {
        Vector3     topEdge = GetHitPoint();
        RaycastHit  vaultRay;
        float       height,
                    space,
                    depth;
        
        // Get height of top of obstacle from ground
        topEdge.y += (obstacle.transform.position.y - topEdge.y) + obstacle.GetComponent<Collider>().bounds.extents.y;
        Physics.Raycast(topEdge, -Vector3.up, out vaultRay);
        height = vaultRay.distance;
        Debug.DrawRay(topEdge, -Vector3.up * vaultRay.distance, vaultDebugColour, 0.01f);

        // Get depth of mesh
        if (obstacle.GetComponent<Collider>().bounds.size.z > obstacle.GetComponent<Collider>().bounds.size.x)
            depth = obstacle.GetComponent<Collider>().bounds.size.z;
        else
            depth = obstacle.GetComponent<Collider>().bounds.size.x;

        ++depth;

        RaycastHit[] hits = Physics.RaycastAll( GetHitPoint() + (transform.forward * depth),
                                                -transform.forward,
                                                depth);

        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject == obstacle)
            {
                depth = (hits[i].point - GetHitPoint()).magnitude;
                i = hits.Length;
            }
        }
        Debug.DrawRay(topEdge, transform.forward * depth, vaultDebugColour, 0.01f);

        // Get space above obstacle
        Physics.Raycast(topEdge + (transform.forward * depth / 2),
                        Vector3.up,
                        out vaultRay);
        if (vaultRay.collider == null)
            space = jumpHeight;
        else
            space = vaultRay.distance;

        // Can vault
        if (height <= crouchingHeight &&
            depth < playerWidth &&
            space >= crouchingHeight)
            return true;

        // Can't vault
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
        Vector3 obstacleBot = GetHitPoint();
        RaycastHit slideRay;
        
        obstacleBot += (transform.forward * obstacle.GetComponent<Collider>().bounds.extents.z);

        // Raycast down from bottom of obstacle to ground
        Physics.Raycast(obstacleBot, -Vector3.up, out slideRay);

        // Raycast up from ground to get distance
        Physics.Raycast(slideRay.point, Vector3.up, out slideRay);
        Debug.DrawLine(slideRay.point - (Vector3.up * slideRay.distance), slideRay.point, slideDebugColour, 0.01f);

        // If distance from ground is more than crouching height
        if (slideRay.distance != 0.0f &&
            slideRay.distance >= crouchingHeight)
        {
            // Can slide
            return true;
        }

        // Can't slide
        return false;
    }
}
