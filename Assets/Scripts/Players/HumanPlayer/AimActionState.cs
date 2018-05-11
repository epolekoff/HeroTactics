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

        if(Input.GetMouseButtonDown(0) && hasValidTarget)
        {
            player.SelectedAction.Execute();
            entity.GetStateMachine().ChangeState(new WatchActionState());
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
}
