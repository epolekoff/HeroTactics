using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimActionState : AbsState {

    /// <summary>
    /// Enter
    /// </summary>
    /// <param name="entity"></param>
    public override void Enter(IStateMachineEntity entity)
    {
        base.Enter(entity);

    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="entity"></param>
    public override void Update(IStateMachineEntity entity)
    {
        HumanPlayer player = (HumanPlayer)entity;

        // If no action is selected, return until an action is selected.
        if(player.SelectedAction == null)
        {
            entity.GetStateMachine().ChangeState(new SelectUnitState());
        }

        // Handle aiming for the selected action.
        bool hasValidTarget = player.SelectedAction.Aim();

        if(Input.GetMouseButtonDown(0))
        {
            if(hasValidTarget)
            {
                player.SelectedAction.Execute(() => OnActionComplete(entity));
            }
            else
            {
                // Clicking on an invalid tile cancels the attack.
                entity.GetStateMachine().ChangeState(new SelectUnitState());
            }
        }
    }

    /// <summary>
    /// Exit
    /// </summary>
    /// <param name="entity"></param>
    public override void Exit(IStateMachineEntity entity)
    {
        base.Exit(entity);
    }

    /// <summary>
    /// Called when the action has finished executing.
    /// </summary>
    private void OnActionComplete(IStateMachineEntity entity)
    {
        // Set that this unit has attacked
        ((HumanPlayer)entity).SelectedUnit.SetHasAttackedThisTurn();

        // Allow the player to select the next unit.
        entity.GetStateMachine().ChangeState(new SelectUnitState());
    }
}
