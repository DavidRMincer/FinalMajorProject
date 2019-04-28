using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extremity : MonoBehaviour
{
    private RaycastHit[]    mainRay;
    private GameObject      surface;
    private float           socketAngle,
                            foreJointAngle,
                            foreBoneLength,
                            rearJointAngle,
                            rearBoneLength;

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
    // Runs on start up
    /////////////////////////////////////////////////////////////////
    private void Start()
    {
        // Set bone lengths
        foreBoneLength = (transform.position - foreBone.transform.position).magnitude;
        rearBoneLength = (foreBone.transform.position - rearBone.transform.position).magnitude;
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
    private void UpdateJoints(Vector3 foreAngle, Vector3 rearAngle)
    {
        foreBone.transform.rotation = Quaternion.Euler(foreAngle);
        rearBone.transform.rotation = Quaternion.Euler(rearAngle);
    }

    /////////////////////////////////////////////////////////////////
    // Returns hypotenuse of joints
    /////////////////////////////////////////////////////////////////
    private Vector3 GetHypotenuse(Vector3 targetPoint)
    {
        Vector3 hypotenuse = targetPoint - rearBone.transform.position;

        // Set values absolute
        hypotenuse.x = Mathf.Abs(hypotenuse.x);
        hypotenuse.y = Mathf.Abs(hypotenuse.y);
        hypotenuse.z = Mathf.Abs(hypotenuse.z);

        return hypotenuse;
    }

    /////////////////////////////////////////////////////////////////
    // Returns opposite of joints
    /////////////////////////////////////////////////////////////////
    private Vector3 GetOpposite()
    {
        Vector3 opposite = foreBone.transform.position - rearBone.transform.position;

        // Set values absolute
        opposite.x = Mathf.Abs(opposite.x);
        opposite.y = Mathf.Abs(opposite.y);
        opposite.z = Mathf.Abs(opposite.z);

        return opposite;
    }

    /////////////////////////////////////////////////////////////////
    // Returns adjacent of joints
    /////////////////////////////////////////////////////////////////
    private Vector3 GetAdjacent()
    {
        Vector3 adjacent = transform.position - rearBone.transform.position;

        // Set values absolute
        adjacent.x = Mathf.Abs(adjacent.x);
        adjacent.y = Mathf.Abs(adjacent.y);
        adjacent.z = Mathf.Abs(adjacent.z);

        return adjacent;
    }

    /////////////////////////////////////////////////////////////////
    // Bends joint to avoid extremity from clipping mesh
    /////////////////////////////////////////////////////////////////
    public void UpdateIK()
    {
        // Only runs if close to surface
        if (surface)
        {
            Vector3 targetPoint = surface.GetComponent<Collider>().ClosestPoint(transform.position);
            


            Debug.DrawLine(transform.position, targetPoint, debugColour, 0.01f);
        }
    }

    /////////////////////////////////////////////////////////////////
    // Moves extremity to contact with mesh
    /////////////////////////////////////////////////////////////////
    public void SnapIK()
    {
        // Only runs if close to surface
        if (surface)
        {
            Vector3 targetPoint = surface.GetComponent<Collider>().ClosestPoint(transform.position),
                    foreAngle = Vector3.zero,
                    rearAngle = Vector3.zero,
                    adj = GetAdjacent(),
                    opp = GetOpposite(),
                    hyp = GetHypotenuse(targetPoint);
            
            Debug.DrawLine(transform.position, targetPoint, debugColour, 0.01f);

            // Calculate fore bone angle using SSS triangle
            foreAngle.x = Mathf.Acos(   ((adj.x * adj.x) + (opp.x * opp.x) - (hyp.x * hyp.x)) /
                                        ((adj.x * opp.x) * 2));
            foreAngle.y = Mathf.Acos(   ((adj.y * adj.y) + (opp.y * opp.y) - (hyp.y * hyp.y)) /
                                        ((adj.y * opp.y) * 2));
            foreAngle.y = Mathf.Acos(   ((adj.z * adj.z) + (opp.z * opp.z) - (hyp.z * hyp.z)) /
                                        (adj.z * opp.z * 2));

            // Calculate rear bone angle using SSS triangle
            rearAngle.x = Mathf.Acos(   ((hyp.x * hyp.x) + (opp.x * opp.x) - (adj.x * adj.x)) /
                                        (hyp.x * opp.x * 2));
            rearAngle.x = Mathf.Acos(   ((hyp.y * hyp.y) + (opp.y * opp.y) - (adj.y * adj.y)) /
                                        (hyp.y * opp.y * 2));
            rearAngle.x = Mathf.Acos(   ((hyp.z * hyp.z) + (opp.z * opp.z) - (adj.z * adj.z)) /
                                        (hyp.z * opp.z * 2));

            // Update joints
            UpdateJoints(foreAngle, rearAngle);
        }
    }
}
