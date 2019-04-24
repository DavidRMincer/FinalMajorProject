using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extremity : MonoBehaviour
{
    private RaycastHit[]    mainRay;
    private GameObject      surface;
    private float           socketAngle,
                            foreJointAngle,
                            rearJointAngle;

    /////////////////////////////////////////////////////////////////

    public GameObject       foreBone,
                            rearBone,
                            socket;
    public float            minSocketAngle,
                            maxSocketAngle,
                            minJointAngle,
                            maxJointAngle,
                            rayLength;
    public Color            debugColour;

    /////////////////////////////////////////////////////////////////
    // Runs while triggered by another collider
    /////////////////////////////////////////////////////////////////
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" &&
            other.tag != "MainCamera")
        {
            // Set collider as obstacle if closest to player
            if ((surface == null) || (surface != null &&
                                        ((other.ClosestPoint(transform.position) - transform.position).magnitude <=
                                        (surface.GetComponent<Collider>().ClosestPoint(transform.position) - transform.position).magnitude))
                )
                surface = other.gameObject;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Runs when collider leaves trigger bounds
    /////////////////////////////////////////////////////////////////
    private void OnTriggerExit(Collider other)
    {
        // Set obstacle to null if collider is obstacle
        if (other.gameObject == surface)
        {
            surface = null;
        }
    }

    /////////////////////////////////////////////////////////////////
    // Updates angles of joints
    /////////////////////////////////////////////////////////////////
    private void UpdateJoints()
    {
        // DO STUFF
    }

    /////////////////////////////////////////////////////////////////
    // Bends joint to avoid extremity from clipping mesh
    /////////////////////////////////////////////////////////////////
    public void UpdateIK()
    {
        Debug.DrawLine(transform.position, surface.GetComponent<Collider>().ClosestPoint(transform.position), debugColour);
        // DO STUFF
    }

    /////////////////////////////////////////////////////////////////
    // Moves extremity to contact with mesh
    /////////////////////////////////////////////////////////////////
    public void SnapIK()
    {
        Debug.DrawLine(transform.position, surface.GetComponent<Collider>().ClosestPoint(transform.position), debugColour);
        // DO STUFF
    }
}
