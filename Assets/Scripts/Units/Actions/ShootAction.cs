using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : UnitAction
{
    /// <summary>
    /// Called each frame during the Aim state.
    /// </summary>
    public override bool Aim()
    {
        return true;
    }

    /// <summary>
    /// Execute this action, using the values gathered while aiming.
    /// </summary>
    public override void Execute()
    {

    }
}
