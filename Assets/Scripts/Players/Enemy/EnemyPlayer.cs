using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayer : Player
{
    // TODO: For now, keep track of the number of enemies moving to end turn when they all stop.
    private int m_numEnemiesMoving;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="myUnits"></param>
    public EnemyPlayer(List<Unit> myUnits)
    {
        SetMyUnits(myUnits);
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
    }

    /// <summary>
    /// Test of movement.
    /// </summary>
    private void MoveAllEnemiesNextToRandomHero()
    {
        foreach(Unit unit in Units)
        {
            if(unit == null)
            {
                continue;
            }

            // Only move ShortRange enemies.
            Enemy enemy = (Enemy)unit;
            if(enemy.EnemyType == EnemyType.ShortRange)
            {
                // Generate a path from one of my units to one of my enemy's.
                GameMap map = GameManager.Instance.Map;
                int randomTargetIndex = UnityEngine.Random.Range(0, GameManager.Instance.HumanPlayer.Units.Count);

                MapTileFilterInfo tileFilterInfo = new MapTileFilterInfo() { NoStoppingOnEnemies = true, NoStoppingOnAllies = true, Player = this };

                List<MapTile> neighborsOfTargetUnit = map.GetValidNeighbors(GameManager.Instance.HumanPlayer.Units[randomTargetIndex].TilePosition, tileFilterInfo);
                if (neighborsOfTargetUnit.Count != 0)
                {
                    MapTile goal = neighborsOfTargetUnit[0];
                    MoveUnitToTile(unit, goal.Position, OnEnemyFinishedMoving);
                    m_numEnemiesMoving++;
                }
            }
        }

        // Check in case no enemies are left to move.
        CheckTurnOver();
    }

    /// <summary>
    /// When all enemies stop moving, end my turn.
    /// </summary>
    private void OnEnemyFinishedMoving(Unit movedUnit)
    {
        m_numEnemiesMoving--;
        CheckTurnOver();
        movedUnit.SetCanMoveAndActAgain();
    }

    /// <summary>
    /// Check if all enemies have finished moving and end the turn.
    /// </summary>
    private void CheckTurnOver()
    {
        if (m_numEnemiesMoving == 0)
        {
            EndTurn();
        }
    }
}
