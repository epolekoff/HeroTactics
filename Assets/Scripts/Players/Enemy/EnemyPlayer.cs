using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyPlayer : Player, IStateMachineEntity
{
    // State machine to watch the units take actions.
    private FiniteStateMachine m_stateMachine;
    public FiniteStateMachine GetStateMachine(int number = 0) { return m_stateMachine; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="myUnits"></param>
    public EnemyPlayer(List<Unit> myUnits)
    {
        SetMyUnits(myUnits);
        m_stateMachine = new FiniteStateMachine(new EnemyPlayerWaitForTurnState(), this);
    }

    // Update is called once per frame
    public override void Update () {
        m_stateMachine.Update();
	}

    /// <summary>
    /// Called when it is my turn again.
    /// </summary>
    public override void StartNewTurn()
    {
        // Just move the enemies up next to the players.
        m_stateMachine.ChangeState(new EnemyPlayerSelectUnitState());
    }

    public override void EndTurn()
    {
        base.EndTurn();

        m_stateMachine.ChangeState(new EnemyPlayerWaitForTurnState());

        // Allow all the units to move again
        foreach(Unit unit in m_myUnits)
        {
            unit.SetCanMoveAndActAgain();
        }
    }

}
