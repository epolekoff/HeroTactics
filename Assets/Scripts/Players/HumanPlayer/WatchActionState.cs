using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchActionState : AbsState
{
    /// <summary>
    /// Enter
    /// </summary>
    /// <param name="entity"></param>
    public override void Enter(IStateMachineEntity entity)
    {
        base.Enter(entity);
        entity.GetStateMachine().ChangeState(new SelectUnitState());
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="entity"></param>
    public override void Update(IStateMachineEntity entity)
    {
        base.Update(entity);

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
