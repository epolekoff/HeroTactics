using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player, IStateMachineEntity
{
    
    public Unit SelectedUnit;

    // Delegates
    public delegate void OnTileClickedDelegate(MapTile tile);

    // Private
    private FiniteStateMachine m_stateMachine;
    public FiniteStateMachine GetStateMachine(int number = 0) { return m_stateMachine; }

    /// <summary>
    /// Constructor
    /// </summary>
    public HumanPlayer(List<Unit> myUnits)
    {
        m_myUnits = myUnits;
        m_stateMachine = new FiniteStateMachine(new SelectUnitState(), this);
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void Update()
    {
        // Update the state machine
        m_stateMachine.Update();

        // Check if the turn is over.
        CheckTurnHasEnded();
    }

    /// <summary>
    /// Check if a tile was clicked this frame and execute a function if it was.
    /// </summary>
    public void CheckClickOnTile(OnTileClickedDelegate callback)
    {
        // Click to select units/move
        if (Input.GetMouseButtonDown(0))
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
                if(mapTile != null)
                {
                    callback(mapTile);
                }
                
            }
        }
    }

    /// <summary>
    /// Called when a new turn is started for me.
    /// </summary>
    public override void StartNewTurn()
    {

    }

    /// <summary>
    /// Check if I've moved all my characters.
    /// </summary>
    private void CheckTurnHasEnded()
    {
        foreach(Unit unit in m_myUnits)
        {
            if(!unit.HasMovedThisTurn)
            {
                return;
            }
        }

        // If we got here, the turn should end.
        EndTurn();
    }

    /// <summary>
    /// Select a specified unit and show its range.
    /// </summary>
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        GameManager.Instance.Map.HighlightMovementRange(unit);

        // Hide the Selected Unit panel
        GameManager.Instance.GameCanvas.ShowUnitStatsPanel(true);
    }

    /// <summary>
    /// Handle when the player clicks off of a valid tile. It should reset state.
    /// </summary>
    public void DeselectUnit()
    {
        // Clear out the selected unit.
        SelectedUnit = null;

        // Clear all highlights
        GameManager.Instance.Map.ClearHighlightedTiles();

        // Hide the Selected Unit panel
        GameManager.Instance.GameCanvas.ShowUnitStatsPanel(false);
    }

    /// <summary>
    /// Move the selected unit to its previous location.
    /// </summary>
    public void UndoMovement()
    {
        if (SelectedUnit == null || !SelectedUnit.HasMovedThisTurn)
        {
            return;
        }

        // Move the unit back to its original tile.
        GameManager.Instance.Map.MoveObjectToTile(SelectedUnit, SelectedUnit.PreviousTilePosition, true);

        // Reset the unit so it can move again.
        SelectedUnit.SetCanMoveAgain();
    }
}
