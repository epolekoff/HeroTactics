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
        // Just move the enemies up next to the players.
        MoveAllEnemiesNextToRandomHero();

        // Pass the turn back to the player, for now.
        EndTurn();
    }

    /// <summary>
    /// Test of movement.
    /// </summary>
    private void MoveAllEnemiesNextToRandomHero()
    {
        foreach(Unit unit in Units)
        {
            // Only move ShortRange enemies.
            Enemy enemy = (Enemy)unit;
            if(enemy.EnemyType == EnemyType.ShortRange)
            {
                // Generate a path from one of my units to one of my enemy's.
                GameMap map = GameManager.Instance.Map;
                int randomTargetIndex = UnityEngine.Random.Range(0, GameManager.Instance.HumanPlayer.Units.Count);
                List<MapTile> neighborsOfTargetUnit = map.GetValidNeighbors(GameManager.Instance.HumanPlayer.Units[randomTargetIndex].TilePosition);
                if (neighborsOfTargetUnit.Count != 0)
                {
                    MapTile goal = neighborsOfTargetUnit[0];
                    MoveUnitToTile(unit, goal.Position);
                }
            }
        }
    }
}
