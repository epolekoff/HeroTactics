using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{

    // Delegates
    public delegate void MapTileDelegate(MapTile tile);

    // Callbacks
    public event MapTileDelegate OnTileClickedEvent;
    public event MapTileDelegate OnMouseOverTileEvent;

    // Private members
    private MapTile m_mousedOverTile = null;

    /// <summary>
    /// Update to poll player input.
    /// </summary>
    void Update()
    {
        CheckMouseOverTile();
        CheckClickOnTile();
    }

    /// <summary>
    /// Get the tile currently under the mouse.
    /// </summary>
    public MapTile GetMousedOverTile()
    {
        return m_mousedOverTile;
    }

    /// <summary>
    /// Check if a tile was clicked this frame and execute a function if it was.
    /// </summary>
    private void CheckClickOnTile()
    {
        // Click to select units/move
        if (Input.GetMouseButtonDown(0) && m_mousedOverTile != null)
        {
            if (OnTileClickedEvent != null)
            {
                OnTileClickedEvent.Invoke(m_mousedOverTile);
            }
        }
    }

    /// <summary>
    /// Raycast at tiles and check if the mouse is over any.
    /// </summary>
    private void CheckMouseOverTile()
    {
        // Click on a tile.
        RaycastHit hit;
        Ray ray = GameManager.Instance.GameCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(
            ray,
            out hit,
            10000f,
            LayerMask.GetMask("MapTile")))
        {
            // Handle clicking on the tile.
            var mapTile = hit.transform.parent.GetComponent<MapTile>();
            if (mapTile != null)
            {
                m_mousedOverTile = mapTile;
                if (OnMouseOverTileEvent != null)
                {
                    OnMouseOverTileEvent.Invoke(mapTile);
                }
                return;
            }
        }
        else
        {
            m_mousedOverTile = null;
        }
    }
}
