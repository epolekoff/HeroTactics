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

public abstract class UnitAction : MonoBehaviour {

    [SerializeField()]
    public string Name;

    [SerializeField()]
    public int Damage;

    [SerializeField()]
    public UnitActionRange Range;

    protected Unit m_owner;

    /// <summary>
    /// Initialize this action when instantiated.
    /// </summary>
    public void Initialize(Unit owner)
    {
        m_owner = owner;
    }

    /// <summary>
    /// Called each frame during the Aim state. This should allow each action to handle aiming itself.
    /// Returns true when the aiming is valid and the player can fire.
    /// </summary>
    public abstract bool Aim();

    /// <summary>
    /// Execute this action, using the values gathered while aiming.
    /// </summary>
    public abstract void Execute();
}
