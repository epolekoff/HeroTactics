using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    // The amount to darken sprites after moving.
    private const float HasMovedDarkenAmount = 0.5f;

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

    /// <summary>
    /// Check if this unit has already moved this turn.
    /// </summary>
    public bool HasMovedThisTurn { get; private set; }

    // Update is called once per frame
    void Update()
    {
        // Keep the sprite billboarded.
        BillboardHeroSprite();
    }

    /// <summary>
    /// Check if this unit can move.
    /// </summary>
    public bool CanMove()
    {
        return !HasMovedThisTurn;
    }

    /// <summary>
    /// Mark that this unit has moved this turn, and dim the visual so we know.
    /// </summary>
    public void SetHasMovedThisTurn()
    {
        HasMovedThisTurn = true;

        // Darken the visual.
        Color c = Visual.GetComponent<SpriteRenderer>().color;
        Visual.GetComponent<SpriteRenderer>().color = new Color(
            c.r - HasMovedDarkenAmount, 
            c.g - HasMovedDarkenAmount, 
            c.b - HasMovedDarkenAmount, 
            c.a);
    }

    /// <summary>
    /// Perform any actions on this unit that should happen when a new turn starts.
    /// </summary>
    public void OnTurnEnd()
    {
        HasMovedThisTurn = false;

        // Re-lighten the visual.
        Color c = Visual.GetComponent<SpriteRenderer>().color;
        Visual.GetComponent<SpriteRenderer>().color = new Color(
            c.r + HasMovedDarkenAmount,
            c.g + HasMovedDarkenAmount,
            c.b + HasMovedDarkenAmount,
            c.a);
    }

    /// <summary>
    /// Keep the sprite facing the camera.
    /// </summary>
    private void BillboardHeroSprite()
    {
        // Look at the camera
        //Visual.transform.LookAt(Camera.main.transform);

        // Keep the sprites Isometric
        Visual.transform.rotation = Camera.main.transform.rotation;
    }
}
