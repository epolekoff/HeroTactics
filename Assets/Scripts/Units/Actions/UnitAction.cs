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


    /// <summary>
    /// Execute this action.
    /// </summary>
    public void Execute(Unit actingUnit)
    {

    }
}
