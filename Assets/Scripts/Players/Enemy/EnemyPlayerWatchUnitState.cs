using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerWatchUnitState : AbsState {

    private Unit m_selectedUnit;
    private EnemyPlayer m_player;

    /// <summary>
    /// Constructor to watch this unit perform its action.
    /// </summary>
    public EnemyPlayerWatchUnitState(Unit selectedUnit)
    {
        m_selectedUnit = selectedUnit;
    }

    /// <summary>
    /// Enter
    /// </summary>
    public override void Enter(IStateMachineEntity entity)
    {
        m_player = (EnemyPlayer)entity;

        // Move the unit to a position next to an opponent.
        MoveUnitNextToOpponent();
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
    /// Move this unit next to a player's unit.
    /// </summary>
    private void MoveUnitNextToOpponent()
    {
        // Only move ShortRange enemies.
        Enemy enemy = (Enemy)m_selectedUnit;
        //if (enemy.EnemyType == EnemyType.ShortRange)
        {
            // Generate a path from one of my units to one of my enemy's.
            MapTile goal = SelectGoalTile(enemy);
            if (goal == null)
            {
                OnEnemyFinishedMoving(enemy);
            }
            m_player.MoveUnitToTile(enemy, goal.Position, OnEnemyFinishedMoving);
        }
    }

    /// <summary>
    /// Select a tile next to an enemy.
    /// </summary>
    /// <returns></returns>
    private MapTile SelectGoalTile(Enemy enemy)
    {
        GameMap map = GameManager.Instance.Map;
        int randomTargetIndex = UnityEngine.Random.Range(0, GameManager.Instance.HumanPlayer.Units.Count);

        MapTileFilterInfo tileFilterInfo = new MapTileFilterInfo() { NoStoppingOnEnemies = true, NoStoppingOnAllies = true, Player = m_player };

        List<MapTile> neighborsOfTargetUnit = map.GetValidNeighbors(GameManager.Instance.HumanPlayer.Units[randomTargetIndex].TilePosition, tileFilterInfo);
        if (neighborsOfTargetUnit.Count != 0)
        {
            MapTile goal = neighborsOfTargetUnit[0];

            // Get a path to the goal, but don't let it go longer than the enemy's movement range.
            var path = Pathfinder.GetPath(map, map.MapTiles[enemy.TilePosition], goal, tileFilterInfo);
            if (enemy.Stats.MovementRange < path.Count)
            {
                path.RemoveRange(enemy.Stats.MovementRange, path.Count - enemy.Stats.MovementRange);
            }

            return path[path.Count - 1];
        }

        return null;
    }

    /// <summary>
    /// When an enemy has finished moving, tell the camera to stop following it.
    /// </summary>
    private void OnEnemyFinishedMoving(Unit movedUnit)
    {
        m_player.GetStateMachine().ChangeState(new EnemyPlayerSelectUnitState());
    }
}
