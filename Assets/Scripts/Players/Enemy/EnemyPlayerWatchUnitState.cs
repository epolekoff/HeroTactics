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

        // Select a goal tile and move there.
        MapTile goal = SelectGoalTile(enemy);
        if (goal == null)
        {
            OnEnemyFinishedMoving(enemy);
            return;
        }
        m_player.MoveUnitToTile(enemy, goal.Position, OnEnemyFinishedMoving);
    }

    /// <summary>
    /// Select a tile next to an enemy.
    /// </summary>
    /// <returns></returns>
    private MapTile SelectGoalTile(Enemy enemy)
    {
        GameMap map = GameManager.Instance.Map;
        int randomTargetIndex = UnityEngine.Random.Range(0, GameManager.Instance.HumanPlayer.Units.Count);

        MapTileFilterInfo tileFilterInfo = new MapTileFilterInfo() { NoStoppingOnEnemies = true, NoStoppingOnAllies = true, AlliesOk = true, Player = m_player };

        List<MapTile> neighborsOfTargetUnit = map.GetValidNeighbors(GameManager.Instance.HumanPlayer.Units[randomTargetIndex].TilePosition, tileFilterInfo);
        if (neighborsOfTargetUnit.Count != 0)
        {
            MapTile goal = neighborsOfTargetUnit[0];
            MapTile start = map.MapTiles[enemy.TilePosition];

            // Get a path to the goal.
            var path = map.GetPath(start, goal, tileFilterInfo, enemy.Stats.MovementRange);
            if(path.Count == 0)
            {
                return null;
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
