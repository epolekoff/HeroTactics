using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player
{
    /// <summary>
    /// Accessor for the units owned by this player.
    /// </summary>
    public List<Unit> Units { get { return m_myUnits; } }

    /// <summary>
    /// Each player has a list of all of their units.
    /// </summary>
    protected List<Unit> m_myUnits = new List<Unit>();

    /// <summary>
    /// Update
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// When the game manager tells this player a new turn has begun.
    /// </summary>
    public abstract void StartNewTurn();

    /// <summary>
    /// Call this from within the player when a turn has ended.
    /// </summary>
    public virtual void EndTurn()
    {
        // Tell each unit the turn has ended.
        foreach(Unit unit in m_myUnits)
        {
            unit.OnTurnEnd();
        }

        // Tell the game manager that the turn has ended.
        GameManager.Instance.EndCurrentPlayersTurn();
    }

    /// <summary>
    /// A player can move a unit to a tile.
    /// </summary>
    public void MoveUnitToTile(Unit unit, Vector3 tilePosition, System.Action OnCompleteCallback = null)
    {
        GameMap map = GameManager.Instance.Map;

        // Move the unit there.
        map.MoveObjectToTile(unit, tilePosition, false, () =>
        {
            unit.SetHasMovedThisTurn();

            // Call additional callbacks
            if (OnCompleteCallback != null)
            {
                OnCompleteCallback();
            }
        });

        // Clear all highlights
        GameManager.Instance.Map.ClearHighlightedTiles();
    }

    /// <summary>
    /// Register all the units as my own.
    /// </summary>
    protected void SetMyUnits(List<Unit> myUnits)
    {
        m_myUnits = myUnits;
        foreach(var unit in myUnits)
        {
            unit.Owner = this;
        }
    }
}
