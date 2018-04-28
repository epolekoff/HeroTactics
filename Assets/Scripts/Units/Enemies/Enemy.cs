using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    /// <summary>
    /// The enemy that this object represents.
    /// </summary>
    public EnemyType EnemyType;

    /// <summary>
    /// Enemies are enemies.
    /// </summary>
    public override bool IsEnemy { get { return true; } }
}
