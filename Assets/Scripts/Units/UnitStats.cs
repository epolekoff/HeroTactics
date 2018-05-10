using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : ScriptableObject
{

    /// <summary>
    /// The name to display for this hero.
    /// </summary>
    [SerializeField()]
    public string DisplayName;

    /// <summary>
    /// Health of this unit.
    /// </summary>
    [SerializeField()]
    public int MaxHealth;

    /// <summary>
    /// Movement range for this unit.
    /// </summary>
    [SerializeField()]
    public int MovementRange;

    /// <summary>
    /// A list of possible actions that this unit can take.
    /// For now, this represents in this level, but could 
    /// represent all possible actions based on skill trees.
    /// </summary>
    [SerializeField()]
    public List<UnitAction> AvailableActions;
}
