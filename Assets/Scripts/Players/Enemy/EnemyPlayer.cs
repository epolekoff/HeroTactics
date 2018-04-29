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
        // Generate a path from one of my units to one of my enemy's.
        GameMap map = GameManager.Instance.Map;
        List<MapTile> neighborsOfTargetUnit = map.GetValidNeighbors(GameManager.Instance.HumanPlayer.Units[0].TilePosition);
        if(neighborsOfTargetUnit.Count != 0)
        {
            MapTile start = map.MapTiles[Units[0].TilePosition];
            MapTile goal = neighborsOfTargetUnit[0];
            Pathfinder.GetPath(start, goal);
        }
        

        // Pass the turn back to the player, for now.
        EndTurn();
    }
}
