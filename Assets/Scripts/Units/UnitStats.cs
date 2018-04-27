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
}
