using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Detector : MonoBehaviour
{
    private float           detectorLength;
    private RaycastHit[]    mainRay;
    private GameObject      obstacle;
    private Vector3         hitPoint;
    private Player_Script   playerScript;

    /////////////////////////////////////////////////////////////////

    public GameObject       playerObject;
    public Color            debugColour,
                            actionColour;

    /////////////////////////////////////////////////////////////////
    // Runs on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        // Set detector length
        detectorLength = GetComponent<Collider>().bounds.size.z;
        
        playerScript = playerObject.GetComponent<Player_Script>();
    }

    /////////////////////////////////////////////////////////////////
    // Runs while triggered by another collider
    /////////////////////////////////////////////////////////////////
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" &&
            other.tag != "MainCamera")
        {
            // Set collider as obstacle if closest to player
            if ((obstacle == null) ||   (obstacle != null &&
                                        ((other.ClosestPoint(playerObject.transform.position) - playerObject.transform.position).magnitude <=
                                        (obstacle.GetComponent<Collider>().ClosestPoint(playerObject.transform.position) - playerObject.transform.position).magnitude))
                )
                obstacle = other.gameObject;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Runs when collider leaves trigger bounds
    /////////////////////////////////////////////////////////////////
    private void OnTriggerExit(Collider other)
    {
        // Set obstacle to null if collider is obstacle
        if (other.gameObject == obstacle)
        {
            obstacle = null;
            playerScript.ResetActions();
        }
    }

    /////////////////////////////////////////////////////////////////
    // Updates gameplay and physics
    /////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        if (obstacle != null)
        {
            Debug.DrawLine(playerObject.transform.position, hitPoint, debugColour, 0.01f);

            // Set hit point
            hitPoint = obstacle.GetComponent<Collider>().ClosestPoint(playerObject.transform.position);

            if (playerScript.GetMovementState() == MovementState.CLIMBING)
            {
                // Mantle after climbing
                if (playerObject.transform.position.y >= (GetTopEdge(obstacle).y - playerScript.GetCrouchHeight()) &&
                    playerObject.transform.position.y <= GetTopEdge(obstacle).y)
                {
                    playerScript.SetMovementState(MovementState.MANTLING);
                }
                // Face obstacle while climbing
                else
                    playerObject.transform.LookAt(hitPoint);
            }

            // Measure obstacle dimensions
            ObstacleCheck(obstacle);

            // Set action point if action started
            if (playerScript.ActionStarted())
                SetActionPoints();
        }
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
    private float GetDepth()
    {
        float depth = 0.0f;

        // Sets depth as longest size axis
        if (obstacle.GetComponent<Collider>().bounds.size.z > obstacle.GetComponent<Collider>().bounds.size.x)
            depth = obstacle.GetComponent<Collider>().bounds.size.z;
        else
            depth = obstacle.GetComponent<Collider>().bounds.size.x;

        // Add extra onto depth for sake of raycasting
        ++depth;

        // Get depth
        mainRay = Physics.RaycastAll(hitPoint + ((hitPoint - playerObject.transform.position).normalized * depth),
                                    -(hitPoint - playerObject.transform.position).normalized,
                                    depth);
        
        // Set depth
        depth = (GetTargetHit(mainRay, obstacle).point - hitPoint).magnitude;
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
    // Returns position of obstacle's top edge
    /////////////////////////////////////////////////////////////////
    private Vector3 GetTopEdge(GameObject obj)
    {
        Vector3 topEdge = hitPoint;
        float topHeight = obj.GetComponent<Collider>().bounds.size.y;

        // Set top edge vector
        ++topHeight;
        mainRay = Physics.RaycastAll(topEdge + (Vector3.up * topHeight), -Vector3.up, topHeight + detectorLength);

        topEdge.y = GetTargetHit(mainRay, obj).point.y;

        return topEdge;
    }

    /////////////////////////////////////////////////////////////////
    // Checks obstacle dimensions and sets action bools
    /////////////////////////////////////////////////////////////////
    private void ObstacleCheck(GameObject obj)
    {
        // Sets all actions to false
        playerObject.GetComponent<Player_Script>().ResetActions();

        Vector3     obstacleBot = hitPoint;
        RaycastHit  actionRay;
        float       topHeight = obstacle.GetComponent<Collider>().bounds.size.y,
                    botHeight,
                    space,
                    depth;
        
        // Get depth of mesh
        depth = GetDepth();

        // Get height from ground
        obstacleBot += (transform.forward * depth / 2);
        mainRay = Physics.RaycastAll(obstacleBot, -Vector3.up, detectorLength);
        obstacleBot = GetFirstHit(mainRay).point;
        mainRay = Physics.RaycastAll(obstacleBot, Vector3.up, detectorLength);
        Debug.DrawLine(obstacleBot, GetFirstHit(mainRay).point, debugColour, 0.001f);
        obstacleBot = GetFirstHit(mainRay).point;
        botHeight = GetFirstHit(mainRay).distance;

        // Get height of top of obstacle from ground
        mainRay = Physics.RaycastAll(GetTopEdge(obstacle), -Vector3.up, obstacle.GetComponent<Collider>().bounds.size.y + detectorLength);
        topHeight = GetFirstHit(mainRay).distance;
        Debug.DrawRay(GetTopEdge(obstacle), -Vector3.up * topHeight, debugColour, 0.001f);

        // Get space above obstacle
        Physics.Raycast(GetTopEdge(obstacle) + (transform.forward * depth / 2),
                        Vector3.up,
                        out actionRay);
        if (actionRay.collider == null)
            space = playerScript.GetJumpHeight();
        else
            space = actionRay.distance;

        Debug.DrawRay(GetTopEdge(obstacle) + (transform.forward * depth / 2), Vector3.up * space, debugColour, 0.001f);

        // Can slide
        playerScript.SetCanSlide(   botHeight > playerScript.GetCrouchHeight() &&
                                    depth < playerScript.GetStandHeight());

        // Can vault
        playerScript.SetCanVault(   topHeight >= playerScript.GetCrouchHeight() &&
                                    topHeight <= playerScript.GetStandHeight() &&
                                    space >= playerScript.GetCrouchHeight() &&
                                    depth < playerScript.GetWidth());

        // Can mantle
        playerScript.SetCanMantle(  topHeight >= playerScript.GetCrouchHeight() &&
                                    topHeight <= playerScript.GetJumpHeight() &&
                                    space > playerScript.GetCrouchHeight() &&
                                    depth >= playerScript.GetWidth());

        // Can climb
        playerScript.SetCanClimb(   topHeight > playerScript.GetJumpHeight() &&
                                    botHeight < playerScript.GetStandHeight());
    }
    
    /////////////////////////////////////////////////////////////////
    // Sets action points based on action
    /////////////////////////////////////////////////////////////////
    private void SetActionPoints()
    {
        switch (playerScript.GetMovementState())
        {
            case MovementState.VAULTING:
                // Set start point
                playerScript.startActionPoint = hitPoint + (-playerObject.transform.forward * (playerScript.GetWidth() / 2));
                playerScript.startActionPoint.y = playerObject.transform.position.y;
                Debug.DrawRay(playerObject.transform.position,
                                playerScript.startActionPoint - playerObject.transform.position,
                                actionColour,
                                playerScript.vaultDuration);

                // Set middle point
                playerScript.middleActionPoint = hitPoint + (playerObject.transform.forward * (GetDepth() / 2));
                playerScript.middleActionPoint.y = GetTopEdge(obstacle).y + (playerScript.GetCrouchHeight() / 2);
                Debug.DrawRay(playerScript.startActionPoint,
                                playerScript.middleActionPoint - playerScript.startActionPoint,
                                actionColour,
                                playerScript.vaultDuration);

                // Set end point
                playerScript.endActionPoint = hitPoint + (playerObject.transform.forward * GetDepth()) + (playerObject.transform.forward * (playerScript.GetWidth() / 2));
                playerScript.endActionPoint.y = playerObject.transform.position.y;
                Debug.DrawRay(playerScript.middleActionPoint,
                                playerScript.endActionPoint - playerScript.middleActionPoint,
                                actionColour,
                                playerScript.vaultDuration);
                break;

            case MovementState.SLIDING:
                // Set start point
                playerScript.startActionPoint = hitPoint + (-playerObject.transform.forward * (playerScript.GetWidth() / 2));
                playerScript.startActionPoint.y = playerObject.transform.position.y;
                Debug.DrawRay(playerObject.transform.position,
                                playerScript.startActionPoint - playerObject.transform.position,
                                actionColour,
                                playerScript.slideDuration);

                // Set end point
                playerScript.endActionPoint = playerScript.startActionPoint + (playerObject.transform.forward * (playerScript.GetWidth() + GetDepth()));
                Debug.DrawRay(playerScript.startActionPoint,
                                playerScript.endActionPoint - playerScript.startActionPoint,
                                actionColour,
                                playerScript.slideDuration);
                break;

            case MovementState.MANTLING:
                // Set start point
                playerScript.startActionPoint = hitPoint + (-playerObject.transform.forward * (playerScript.GetWidth() / 2));
                playerScript.startActionPoint.y = playerObject.transform.position.y;
                Debug.DrawRay(playerObject.transform.position,
                                playerScript.startActionPoint - playerObject.transform.position,
                                actionColour,
                                playerScript.mantleDuration);

                // Set middle point
                playerScript.middleActionPoint = GetTopEdge(obstacle) + (-playerObject.transform.forward * (playerScript.GetWidth() / 2));
                playerScript.middleActionPoint -= Vector3.up * (playerScript.GetCrouchHeight() / 2);
                Debug.DrawRay(playerScript.startActionPoint,
                                playerScript.middleActionPoint - playerScript.startActionPoint,
                                actionColour,
                                playerScript.mantleDuration);

                // Set end point
                playerScript.endActionPoint = GetTopEdge(obstacle) + (playerObject.transform.forward * playerScript.GetWidth());
                playerScript.endActionPoint += Vector3.up * (playerScript.GetCrouchHeight() / 2);
                Debug.DrawRay(playerScript.middleActionPoint,
                                playerScript.endActionPoint - playerScript.middleActionPoint,
                                actionColour,
                                playerScript.mantleDuration);
                break;

            case MovementState.CLIMBING:
                // Set start point
                playerScript.startActionPoint = hitPoint - (playerObject.transform.forward * playerScript.GetWidth() / 4);
                playerScript.startActionPoint.y = hitPoint.y + playerScript.GetCrouchHeight();
                Debug.DrawRay(  playerScript.transform.position,
                                playerScript.startActionPoint - playerScript.transform.position,
                                actionColour,
                                playerScript.slideDuration);
                break;

            default:
                break;
        }
        // DO STUFF
    }
}
