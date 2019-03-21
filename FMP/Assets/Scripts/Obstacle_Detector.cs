using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Detector : MonoBehaviour
{
    private float       standingHeight,
                        crouchingHeight,
                        jumpHeight,
                        playerWidth;

    public float        detectorLength;
    public GameObject   detectorBox;

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
        // DO STUFF
        return false;
    }
}
