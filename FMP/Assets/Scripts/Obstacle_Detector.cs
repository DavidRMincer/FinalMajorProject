using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Detector : MonoBehaviour
{
    private float           standingHeight,
                            crouchingHeight,
                            jumpHeight,
                            playerWidth;
    private RaycastHit[]    mainRay;
    private GameObject      obstacle;
    private Vector3         hitPoint;

    public GameObject       playerObject;
    public float            detectorLength;
    public Color            debugColour;

    private void Start()
    {
        SetDimensions();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" &&
            other.tag != "MainCamera")
        {
            // Set hit point
            hitPoint = other.ClosestPoint(playerObject.transform.position);

            mainRay = Physics.RaycastAll( playerObject.transform.position,
                                                    (hitPoint - playerObject.transform.position).normalized,
                                                    detectorLength);

            if (GetFirstHit(mainRay).collider == other)
            {
                obstacle = other.gameObject;
                Debug.DrawRay(playerObject.transform.position,
                                hitPoint - playerObject.transform.position,
                                debugColour,
                                0.016f);
                
                ObstacleCheck(obstacle);
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
    // Returns first collider hit by main ray
    /////////////////////////////////////////////////////////////////
    private RaycastHit GetFirstHit(RaycastHit[] rayArray)
    {
        // Check array of hit points
        for (int i = 0; i < rayArray.Length; ++i)
        {
            // Set hit object as first none player object
            if (rayArray[i].collider.tag != "Player" &&
                rayArray[i].collider.tag != "MainCamera")
            {
                // Return hit
                return rayArray[i];
            }
        }

        // Return first hit
        return new RaycastHit();
    }

    /////////////////////////////////////////////////////////////////
    // Returns hit on target
    /////////////////////////////////////////////////////////////////
    private RaycastHit GetTargetHit(RaycastHit[] rayArray, GameObject target)
    {
        // Check array of hit points
        for (int i = 0; i < rayArray.Length; ++i)
        {
            // Set hit object as target object
            if (rayArray[i].collider.gameObject == target)
            {
                // Return hit
                return rayArray[i];
            }
        }

        // Return first hit
        return new RaycastHit();
    }

    /////////////////////////////////////////////////////////////////
    // Returns depth of closest section of obstacle
    /////////////////////////////////////////////////////////////////
    private float GetDepth(GameObject obj)
    {
        float depth;

        // Sets depth as longest size axis
        if (obj.GetComponent<Collider>().bounds.size.z > obstacle.GetComponent<Collider>().bounds.size.x)
            depth = obj.GetComponent<Collider>().bounds.size.z;
        else
            depth = obj.GetComponent<Collider>().bounds.size.x;

        // Add extra onto depth for sake of raycasting
        ++depth;

        // Get depth
        mainRay = Physics.RaycastAll(hitPoint + (transform.forward * depth),
                                    -transform.forward,
                                    depth);

        if (GetFirstHit(mainRay).collider.gameObject == obj)
            depth = (GetFirstHit(mainRay).point - hitPoint).magnitude;
        Debug.DrawRay(hitPoint, transform.forward * depth, debugColour, 0.01f);

        return depth;
    }

    /////////////////////////////////////////////////////////////////
    // Returns height of closest section of obstacle
    /////////////////////////////////////////////////////////////////
    private float GetHeight(GameObject obj)
    {
        float   height = obj.GetComponent<Collider>().bounds.size.y;
        Vector3 topEdge = hitPoint;

        // Set top edge vector
        ++height;
        mainRay = Physics.RaycastAll(hitPoint + (Vector3.up * height), -Vector3.up, height + detectorLength);

        topEdge.y = GetTargetHit(mainRay, obj).point.y;

        // Get height of top of obstacle from ground
        mainRay = Physics.RaycastAll(topEdge, -Vector3.up, height + detectorLength);
        height = GetFirstHit(mainRay).distance;
        Debug.DrawRay(topEdge, -Vector3.up * height, debugColour, 0.01f);

        return height;
    }

    /////////////////////////////////////////////////////////////////
    // Checks obstacle dimensions and sets action bools
    /////////////////////////////////////////////////////////////////
    private void ObstacleCheck(GameObject obj)
    {
        // Sets all actions to false
        playerObject.GetComponent<Player_Script>().ResetActions();

        Vector3     topEdge = hitPoint,
                    obstacleBot = hitPoint;
        RaycastHit  actionRay;
        float       topHeight = obstacle.GetComponent<Collider>().bounds.size.y,
                    botHeight,
                    space,
                    depth;

        // Set top edge vector
        ++topHeight;
        mainRay = Physics.RaycastAll(topEdge + (Vector3.up * topHeight), -Vector3.up, topHeight + detectorLength);

        topEdge.y = GetTargetHit(mainRay, obstacle).point.y;

        // Get depth of mesh
        depth = GetDepth(obstacle);

        // Get height from ground
        obstacleBot += (transform.forward * GetDepth(obstacle) / 2);
        mainRay = Physics.RaycastAll(obstacleBot, -Vector3.up, detectorLength);
        obstacleBot = GetFirstHit(mainRay).point;
        mainRay = Physics.RaycastAll(obstacleBot, Vector3.up, detectorLength);
        Debug.DrawLine(obstacleBot, GetFirstHit(mainRay).point, debugColour, 0.01f);
        obstacleBot = GetFirstHit(mainRay).point;
        botHeight = GetFirstHit(mainRay).distance;

        // Get height of top of obstacle from ground
        Physics.Raycast(topEdge, -Vector3.up, out actionRay);
        topHeight = actionRay.distance;
        Debug.DrawRay(topEdge, -Vector3.up * topHeight, debugColour, 0.01f);
        
        // Get space above obstacle
        Physics.Raycast(topEdge + (transform.forward * depth / 2),
                        Vector3.up,
                        out actionRay);
        if (actionRay.collider == null)
            space = jumpHeight;
        else
            space = actionRay.distance;

        Debug.DrawRay(topEdge + (transform.forward * depth / 2), Vector3.up * space, debugColour, 0.01f);
        
        // Can slide
        GetComponent<Player_Script>().SetCanSlide(  botHeight > crouchingHeight);

        // Can vault
        GetComponent<Player_Script>().SetCanVault(  topHeight >= crouchingHeight &&
                                                    space > crouchingHeight &&
                                                    depth < playerWidth);

        // Can mantle
        GetComponent<Player_Script>().SetCanMantle( topHeight <= jumpHeight &&
                                                    space > crouchingHeight &&
                                                    depth >= playerWidth);

        // Can climb
        GetComponent<Player_Script>().SetCanClimb(  topHeight > jumpHeight &&
                                                    botHeight < standingHeight);
    }
}
