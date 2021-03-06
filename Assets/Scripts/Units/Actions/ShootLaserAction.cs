﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class ShootLaserAction : UnitAction
{
    private List<Unit> m_hitUnits;

    public override bool Aim()
    {
        // Clear the list of hit units.
        ClearHitUnitTileHighlights();
        m_hitUnits = new List<Unit>();

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
                // Cast a ray, get all of the units and walls hit.
                RaycastHit[] hits = Physics.RaycastAll(
                    new Ray(lastHitPoint, aimVector),
                    rangeRemaining,
                    LayerMask.GetMask("MapTile", "Unit")).OrderBy(h => h.distance).ToArray(); ;

                // If we hit something, reflect off of the first wall and record all enemies hit.
                if(hits.Length > 0)
                {
                    bool mapTileWasHitThisRaycast = false;
                    for(int i = 0; i < hits.Length; i++)
                    {
                        Unit hitUnit = hits[i].transform.GetComponent<Unit>();
                        MapTile hitTile = hits[i].transform.GetComponent<MapTile>();

                        // If hitting a unit, add it to a list to deal damage.
                        if (hitUnit && !m_hitUnits.Contains(hitUnit))
                        {
                            m_hitUnits.Add(hits[i].transform.GetComponent<Unit>());
                            continue;
                        }

                        // If hitting a tile, reflect the laser and break. Don't pierce walls.
                        if(hitTile)
                        {
                            mapTileWasHitThisRaycast = true;

                            // Keep track of the distance to this point and subtract it from the range.
                            float distanceToHit = Vector3.Distance(unitPosition, hits[i].point);
                            rangeRemaining -= distanceToHit;

                            // Keep track of the last hit point.
                            lastHitPoint = hits[i].point;

                            // Add the hit point.
                            aimLinePoints.Add(hits[i].point);

                            // Update the aim vector.
                            aimVector = Vector3.Reflect(aimVector, hits[i].normal);
                            break;
                        }
                    }

                    // If we cast through a bunch of objects and no wall was hit, then we don't need to bounce off of anything.
                    if(!mapTileWasHitThisRaycast)
                    {
                        // Add the last point to the list
                        aimLinePoints.Add(lastHitPoint + aimVector * rangeRemaining);

                        doneBouncing = true;
                    }
                }
                else
                {
                    // Add the last point to the list
                    aimLinePoints.Add(lastHitPoint + aimVector * rangeRemaining);

                    doneBouncing = true;
                }
            }

            // For each unit hit, highlight their tile tile.
            foreach (Unit hitUnit in m_hitUnits)
            {
                GameManager.Instance.Map.MapTiles[hitUnit.TilePosition].ShowHighlight(HighlightState.TargetEnemy);
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

        // Hit all of the enemies.
        foreach (Unit hitUnit in m_hitUnits)
        {
            hitUnit.TakeDamage(Damage);
        }

        // Clear the Hit Unit tile highlights.
        ClearHitUnitTileHighlights();

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

        // Clear the Hit Unit tile highlights.
        ClearHitUnitTileHighlights();

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

    /// <summary>
    /// Clear the tiles of the highlighted units.
    /// </summary>
    private void ClearHitUnitTileHighlights()
    {
        if(m_hitUnits == null)
        {
            return;
        }

        foreach (Unit hitUnit in m_hitUnits)
        {
            GameManager.Instance.Map.MapTiles[hitUnit.TilePosition].ShowHighlight(HighlightState.None);
        }
    }
}
