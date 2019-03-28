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
    private RaycastHit  mainRay;
    private GameObject  obstacle;
    private rayHit      currentRayHit;
    private Vector3     hitPoint;

    public GameObject   playerObject;
    public float        detectorLength;
    public Color        debugColour,
                        slideDebugColour,
                        vaultDebugColour,
                        mantleDebugColour,
                        climbDebugColour;

    private void Start()
    {
        SetDimensions();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" &&
            other.tag != "MainCamera")
        {
            obstacle = other.gameObject;
            RaycastHit[] hits = Physics.RaycastAll( playerObject.transform.position,
                                                    (obstacle.transform.position - playerObject.transform.position).normalized,
                                                    detectorLength);
            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.gameObject == obstacle)
                {
                    hitPoint = hits[i].point;
                    Debug.DrawRay(playerObject.transform.position, hitPoint - playerObject.transform.position, debugColour, 0.016f);
                    i = hits.Length;
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////
    // Sets dimensions from player and detector box dimensions
    /////////////////////////////////////////////////////////////////
    private void SetDimensions()
    {
        // Set standing height as player height
        standingHeight = playerObject.GetComponent<Collider>().bounds.size.y;
        // Set crouching height as half player height
        crouchingHeight = standingHeight / 2;
        // Set player width as player width
        playerWidth = playerObject.GetComponent<Collider>().bounds.size.z;
        // Set jump height
        jumpHeight = standingHeight * 2;

        // Set detector length
        detectorLength = GetComponent<Collider>().bounds.size.z;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if a ray hits an obstacle
    /////////////////////////////////////////////////////////////////
    private bool HitObstacle()
    {
        //currentRayHit = rayHit.NONE;

        //// Middle raycast hits
        //if (midHit.collider != null && midHit.distance <= detectorLength && midHit.distance != 0.0f)
        //{
        //    obstacle = midHit.collider.gameObject;
        //    currentRayHit = rayHit.MID;
        //    return true;
        //}
        //// Bottom raycast hits
        //else if (botHit.collider != null && botHit.distance <= detectorLength && botHit.distance != 0.0f)
        //{
        //    obstacle = botHit.collider.gameObject;
        //    currentRayHit = rayHit.BOT;
        //    return true;
        //}
        //// Top raycast hits
        //else if (topHit.collider != null && topHit.distance <= detectorLength && topHit.distance != 0.0f)
        //{
        //    obstacle = topHit.collider.gameObject;
        //    currentRayHit = rayHit.TOP;
        //    return true;
        //}
        //// Upper raycast hits
        //else if (upperHit.collider != null && upperHit.distance <= detectorLength && upperHit.distance != 0.0f)
        //{
        //    obstacle = upperHit.collider.gameObject;
        //    currentRayHit = rayHit.UPPER;
        //    return true;
        //}

        return false;
    }

    /////////////////////////////////////////////////////////////////
    // Returns position of point where 
    /////////////////////////////////////////////////////////////////
    private Vector3 GetHitPoint()
    {
        // return hit point from the raycast that hit obstacle
        //switch (currentRayHit)
        //{
        //    case rayHit.BOT:
        //        return botHit.point;
        //    case rayHit.MID:
        //        return midHit.point;
        //    case rayHit.TOP:
        //        return topHit.point;
        //    case rayHit.UPPER:
        //        return upperHit.point;
        //    default:
        //        return obstacle.transform.position;
        //}
        return Vector3.zero;
    }

    /////////////////////////////////////////////////////////////////
    // Returns true if player can vault obstacle
    /////////////////////////////////////////////////////////////////
    public bool CanVault()
    {
        Vector3         topEdge = GetHitPoint();
        RaycastHit      vaultRay;
        float           height = obstacle.GetComponent<Collider>().bounds.size.y,
                        space,
                        depth;
        RaycastHit[]    hits;

        // Set top edge vector
        ++height;
        hits = Physics.RaycastAll(topEdge + (Vector3.up * height), -Vector3.up, height + detectorLength);

        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject == obstacle)
            {
                topEdge.y = hits[i].point.y;
                i = hits.Length;
            }
        }

        // Get height of top of obstacle from ground
        Physics.Raycast(topEdge, -Vector3.up, out vaultRay);
        height = vaultRay.distance;
        Debug.DrawRay(topEdge, -Vector3.up * vaultRay.distance, vaultDebugColour, 0.01f);

        // Get depth of mesh
        if (obstacle.GetComponent<Collider>().bounds.size.z > obstacle.GetComponent<Collider>().bounds.size.x)
            depth = obstacle.GetComponent<Collider>().bounds.size.z;
        else
            depth = obstacle.GetComponent<Collider>().bounds.size.x;

        ++depth;

        hits = Physics.RaycastAll( GetHitPoint() + (transform.forward * depth),
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

        Debug.DrawRay(topEdge + (transform.forward * depth / 2), Vector3.up * space, vaultDebugColour, 0.01f);

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
