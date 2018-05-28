using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShootLaserAction : UnitAction
{

    public override bool Aim()
    {
        // Get the unit position
        Vector3 unitPosition = m_owner.transform.position + Vector3.up * 0.1f;

        // Draw a line showing where we're aiming.
        GetComponent<LineRenderer>().enabled = true;
        List<Vector3> aimLinePoints = new List<Vector3>();
        aimLinePoints.Add(unitPosition);

        // Raycast from the mouse to get a point on the tiles.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a plane at the unit's position to detect the mouse raycast.
        Plane unitPlane = new Plane(Vector3.up, unitPosition);

        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;
        if (unitPlane.Raycast(ray, out distance))
        {
            // Get the hit point and calculate the aim vector.
            Vector3 aimVector = (ray.GetPoint(distance) - unitPosition).normalized;

            // Raycast along the aim vector to see if the laser will bounce off of any walls.
            bool doneBouncing = false;
            Vector3 lastHitPoint = unitPosition;
            float rangeRemaining = RangeValue;
            while(!doneBouncing)
            {
                RaycastHit hit;
                if (Physics.Raycast(
                    new Ray(lastHitPoint, aimVector),
                    out hit,
                    rangeRemaining,
                    LayerMask.GetMask("MapTile")))
                {
                    // Keep track of the distance to this point and subtract it from the range.
                    float distanceToHit = Vector3.Distance(unitPosition, hit.point);
                    rangeRemaining -= distanceToHit;

                    aimVector = Vector3.Reflect(aimVector, hit.normal);
                    lastHitPoint = hit.point;

                    // Add the hit point.
                    aimLinePoints.Add(hit.point);

                }
                else
                {
                    // Add the last point to the list
                    aimLinePoints.Add(lastHitPoint + aimVector * rangeRemaining);

                    doneBouncing = true;
                }
            }
        }

        GetComponent<LineRenderer>().positionCount = aimLinePoints.Count;
        GetComponent<LineRenderer>().SetPositions(aimLinePoints.ToArray());

        return true;
    }

    /// <summary>
    /// Fire the laser.
    /// </summary>
    public override void Execute(OnExecuteComplete callback)
    {
        // Hide the aiming line.
        GetComponent<LineRenderer>().enabled = false;

        // Change states.
        if (callback != null)
        {
            callback();
        }
    }

    /// <summary>
    /// Cancel the aiming of the laser.
    /// </summary>
    public override void Cancel(System.Action callback)
    {
        // Hide the aiming line.
        GetComponent<LineRenderer>().enabled = false;

        // Change states.
        if (callback != null)
        {
            callback();
        }
    }

    public override void OnActionSelected(Unit selectedUnit)
    {

    }

    public override void OnActionDeselected(Unit selectedUnit)
    {
        
    }
}
