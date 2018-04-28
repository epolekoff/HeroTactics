using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUnitState : AbsState
{
    private Player m_player;

    /// <summary>
    /// Enter
    /// </summary>
    /// <param name="entity"></param>
    public override void Enter(IStateMachineEntity entity)
    {
        base.Enter(entity);

        // Store the player for ease of access.
        m_player = ((Player)entity);
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="entity"></param>
    public override void Update(IStateMachineEntity entity)
    {
        base.Enter(entity);

        // Check if the player clicked on a tile, and handle it if they did.
        ((Player)entity).CheckClickOnTile(OnTileClicked);
    }

    /// <summary>
    /// Exit
    /// </summary>
    /// <param name="entity"></param>
    public override void Exit(IStateMachineEntity entity)
    {
        base.Enter(entity);
    }


    /// <summary>
    /// Handle clicking on a tile.
    /// </summary>
    private void OnTileClicked(MapTile tile)
    {
        if(tile == null)
        {
            return;
        }

        GameMap map = GameManager.Instance.Map;
        Unit unitOnTile = map.GetUnitOnTile(tile.Position);

        // If clicking a tile with a unit, select the unit
        if (unitOnTile != null)
        {
            m_player.SelectUnit(unitOnTile);
        }
        // If clicking a blue highlight, move there.
        else if (tile.HighlightState == HighlightState.Friendly && m_player.SelectedUnit != null)
        {
            // Move there
            map.MoveObjectToTile(m_player.SelectedUnit, tile.Position, false, null);

            // Clear all highlights
            GameManager.Instance.Map.ClearHighlightedTiles();
        }
        // If clicking any other tile, deselect the unit and hide the move range.
        else
        {
            m_player.DeselectUnit();
        }
    }
}
