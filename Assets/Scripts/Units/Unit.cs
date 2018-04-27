using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour{

    /// <summary>
    /// The sprite representing this character.
    /// </summary>
    public GameObject Visual;

    /// <summary>
    /// Stats for this character.
    /// </summary>
    public UnitStats Stats;

    /// <summary>
    /// The position of this unit on the map.
    /// </summary>
    public Vector3 TilePosition;

    /// <summary>
    /// Is this unit an enemy?
    /// </summary>
    public virtual bool IsEnemy { get; set; }
}
