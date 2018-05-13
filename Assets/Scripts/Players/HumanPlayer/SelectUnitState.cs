using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUnitState : AbsState
{
    private HumanPlayer m_player;

    /// <summary>
    /// Enter
    /// </summary>
    /// <param name="entity"></param>
    public override void Enter(IStateMachineEntity entity)
    {
        base.Enter(entity);

        // Store the player for ease of access.
        m_player = ((HumanPlayer)entity);

        // Register the click callback.
        InputManager.Instance.OnTileClickedEvent += OnTileClicked;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="entity"></param>
    public override void Update(IStateMachineEntity entity)
    {
        base.Enter(entity);
    }

    /// <summary>
    /// Exit
    /// </summary>
    /// <param name="entity"></param>
    public override void Exit(IStateMachineEntity entity)
    {
        base.Exit(entity);

        // Remove the callback
        InputManager.Instance.OnTileClickedEvent -= OnTileClicked;
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
        if (unitOnTile != null && unitOnTile.CanAttack())
        {
            m_player.SelectUnit(unitOnTile);
        }
        // If clicking a blue highlight, move there.
        else if (tile.HighlightState == HighlightState.Friendly && m_player.SelectedUnit != null)
        {
            // Move the selected unit to the tile.
            m_player.MoveUnitToTile(m_player.SelectedUnit, tile.Position, null);

            // Watch the unit move.
            m_player.GetStateMachine().ChangeState(new WatchUnitMoveState());
        }
        // If clicking any other tile, deselect the unit and hide the move range.
        else
        {
            m_player.DeselectUnit();
        }
    }
}
