using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClickAttackAction : UnitAction
{
    // Members
    protected MapTile m_targetTile;

    /// <summary>
    /// Called when this action is selected.
    /// Allows each action to set up UI for its aiming.
    /// </summary>
    public override void OnActionSelected(Unit selectedUnit)
    {
        MapTileFilterInfo tileFilterInfo = new MapTileFilterInfo()
        {
            EnemiesOk = true,
            AlliesOk = true,
            NoStoppingOnAllies = true
        };
        GameManager.Instance.Map.HighlightActionRange(selectedUnit.TilePosition, Range, tileFilterInfo);
    }

    /// <summary>
    /// Called when this action is deselected, cancelled, or completed properly.
    /// </summary>
    public override void OnActionDeselected(Unit selectedUnit)
    {
        GameManager.Instance.Map.ClearHighlightedTiles();
    }

    /// <summary>
    /// Called each frame during the Aim state.
    /// </summary>
    public override bool Aim()
    {
        MapTile mousedOverTile = InputManager.Instance.GetMousedOverTile();
        if (mousedOverTile == null)
        {
            return false;
        }

        // Check if the mouse is hovering over a tile highlighted in the attack state.
        if (mousedOverTile.HighlightState == HighlightState.Attack)
        {
            m_targetTile = mousedOverTile;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Execute this action, using the values gathered while aiming.
    /// </summary>
    public override void Execute(OnExecuteComplete callback)
    {
        Unit attackedUnit = GameManager.Instance.Map.GetUnitOnTile(m_targetTile.Position);
        if(attackedUnit != null)
        {
            attackedUnit.TakeDamage(Damage);
        }

        // Clean up the highlights.
        GameManager.Instance.Map.ClearHighlightedTiles();

        // Play any animations or whatever, and then call the callback
        m_owner.StartCoroutine(WaitForAttackToFinish(callback));
    }

    /// <summary>
    /// For now, we need to have some delay time before switching to another state.
    /// This will be replaced with an actual animation later.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForAttackToFinish(OnExecuteComplete callback)
    {
        yield return new WaitForSeconds(0.1f);

        if (callback != null)
        {
            callback();
        }
    }
}
