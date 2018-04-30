using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    /// <summary>
    /// The hero that this object represents.
    /// </summary>
    public HeroType HeroType;

    /// <summary>
    /// Heroes are not enemies.
    /// </summary>
    public override bool IsEnemy {  get { return false; } }
}
