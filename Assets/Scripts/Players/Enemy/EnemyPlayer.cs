using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayer : Player
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="myUnits"></param>
    public EnemyPlayer(List<Unit> myUnits)
    {
        m_myUnits = myUnits;
    }

    // Update is called once per frame
    public override void Update () {
		
	}

    /// <summary>
    /// Called when it is my turn again.
    /// </summary>
    public override void StartNewTurn()
    {
        // Pass the turn back to the player, for now.
        EndTurn();
    }
}
