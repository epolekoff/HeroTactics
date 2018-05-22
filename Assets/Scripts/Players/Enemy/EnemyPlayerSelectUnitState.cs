using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyPlayerSelectUnitState : AbsState
{
    private EnemyPlayer m_player;

    private Unit m_selectedUnit;

    /// <summary>
    /// Enter
    /// </summary>
    public override void Enter(IStateMachineEntity entity)
    {
        m_player = (EnemyPlayer)entity;
        SelectUnitAndTransitionCamera(m_player);
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void Update(IStateMachineEntity entity)
    {

    }

    /// <summary>
    /// Exit
    /// </summary>
    public override void Exit(IStateMachineEntity entity)
    {

    }

    /// <summary>
    /// Sort the remaining units by priority and take the next unit's turn.
    /// </summary>
    private void SelectUnitAndTransitionCamera(EnemyPlayer player)
    {
        m_selectedUnit = GetNextUnitThisTurn(player);

        // If there are no remaining units, then this turn is over.
        if (m_selectedUnit == null)
        {
            player.EndTurn();
            return;
        }

        // Move the camera to where it needs to go.
        GameManager.Instance.GameCamera.FocusOnTile(GameManager.Instance.Map.MapTiles[m_selectedUnit.TilePosition], OnCameraFinishedMovingToSelectedUnit);
    }

    /// <summary>
    /// Given all of my units, determine which ones will have a meaningful turn, and move them first.
    /// This will prevent the camera from jumping around to the backlines first.
    /// </summary>
    private Unit GetNextUnitThisTurn(EnemyPlayer player)
    {
        // Sort the units.
        List<Unit> remainingUnits = player.Units.Where(u => u.CanMove()).ToList();

        // Return the next unit who has not moved
        return remainingUnits.FirstOrDefault();
    }

    /// <summary>
    /// When the camera has reached the unit in question, transition to the 
    /// next state and watch the unit perform the action.
    /// </summary>
    private void OnCameraFinishedMovingToSelectedUnit()
    {
        m_player.GetStateMachine().ChangeState(new EnemyPlayerWatchUnitState(m_selectedUnit));
    }

}
