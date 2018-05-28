using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player, IStateMachineEntity
{
    
    public Unit SelectedUnit;
    public UnitAction SelectedAction;

    // Private
    private FiniteStateMachine m_stateMachine;
    public FiniteStateMachine GetStateMachine(int number = 0) { return m_stateMachine; }

    /// <summary>
    /// Constructor
    /// </summary>
    public HumanPlayer(List<Unit> myUnits)
    {
        SetMyUnits(myUnits);
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
    /// Called when a new turn is started for me.
    /// </summary>
    public override void StartNewTurn()
    {
        GameManager.Instance.GameCamera.FocusOnTile(GameManager.Instance.Map.MapTiles[m_myUnits[0].TilePosition]);
    }

    /// <summary>
    /// Check if I've moved all my characters.
    /// </summary>
    private void CheckTurnHasEnded()
    {
        foreach(Unit unit in m_myUnits)
        {
            if(!unit.HasAttackedThisTurn)
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

        // When selecting a unit, clear the highlighted tiles.
        GameManager.Instance.Map.ClearHighlightedTiles();

        // Highlight the movement range, if the unit can move.
        if (unit.CanMove())
        {
            GameManager.Instance.Map.HighlightMovementRange(unit);
        }

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
        GameManager.Instance.Map.MoveObjectToTile(SelectedUnit, SelectedUnit.PreviousTilePosition, forceImmediate: true);

        // Reset the unit so it can move again.
        SelectedUnit.SetCanMoveAndActAgain();
    }

    /// <summary>
    /// Select the action at the specified index for the selected unit
    /// </summary>
    public void SelectAction(int index)
    {
        if(SelectedUnit == null || SelectedUnit.IsEnemyOf(this) || !SelectedUnit.CanAttack())
        {
            return;
        }

        if(index < 0 || index >= SelectedUnit.AvailableActions.Count)
        {
            Debug.LogError(string.Format("Attempting to select an action with an out-of-bound index {0} for unit {1}.", index, SelectedUnit.Stats.DisplayName));
            return;
        }

        // Hide existing highlights before creating new ones for the aiming.
        GameManager.Instance.Map.ClearHighlightedTiles();

        // Select an action, and deselect the old action.
        if (SelectedAction != null) { SelectedAction.OnActionDeselected(SelectedUnit); }
        SelectedAction = SelectedUnit.AvailableActions[index];
        SelectedAction.OnActionSelected(SelectedUnit);

        // Now that an action is selected, allow the player to aim it.
        GetStateMachine().ChangeState(new AimActionState());
    }
}
