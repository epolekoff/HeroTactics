using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitActionRange
{
    Invalid,
    Self,
    Adjacent,
    SkipOneTile,
}

public abstract class UnitAction : MonoBehaviour
{
    // Delegates
    public delegate void OnExecuteComplete();

    [SerializeField()]
    public string Name;

    [SerializeField()]
    public int Damage;

    [SerializeField()]
    public UnitActionRange Range;

    [SerializeField()]
    public int RangeValue;

    protected Unit m_owner;

    /// <summary>
    /// Initialize this action when instantiated.
    /// </summary>
    public void Initialize(Unit owner)
    {
        m_owner = owner;
    }

    /// <summary>
    /// Called when this action is selected.
    /// Allows each action to set up UI for its aiming.
    /// </summary>
    public abstract void OnActionSelected(Unit selectedUnit);

    /// <summary>
    /// Called when this action is deselected, cancelled, or completed properly.
    /// </summary>
    public abstract void OnActionDeselected(Unit selectedUnit);

    /// <summary>
    /// Called each frame during the Aim state. This should allow each action to handle aiming itself.
    /// Returns true when the aiming is valid and the player can fire.
    /// </summary>
    public abstract bool Aim();

    /// <summary>
    /// Execute this action, using the values gathered while aiming.
    /// </summary>
    public abstract void Execute(OnExecuteComplete callback);
}
