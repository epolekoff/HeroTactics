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
    /// Health for the unit
    /// </summary>
    public int CurrentHealth { get; protected set; }

    /// <summary>
    /// The position of this unit on the map.
    /// </summary>
    public Vector3 TilePosition = new Vector3(int.MinValue, int.MinValue, int.MinValue);

    /// <summary>
    /// The previous position of this unit, so the player can undo their move.
    /// </summary>
    public Vector3 PreviousTilePosition;

    /// <summary>
    /// Is this unit an enemy?
    /// </summary>
    public virtual bool IsEnemy { get; set; }

    public bool IsMoving { get; set; }

    /// <summary>
    /// Check if this unit has already acted this turn.
    /// </summary>
    public bool HasMovedThisTurn { get; private set; }
    public bool HasAttackedThisTurn { get; private set; }

    public List<UnitAction> AvailableActions { get; private set; }

    /// <summary>
    /// Set up this unit when it's created.
    /// </summary>
    public void Initialize()
    {
        CurrentHealth = Stats.MaxHealth;
        AvailableActions = Stats.AvailableActions;
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the sprite billboarded.
        BillboardHeroSprite();
    }

    /// <summary>
    /// Check if this unit can attack.
    /// </summary>
    public bool CanAttack()
    {
        return !HasAttackedThisTurn;
    }

    /// <summary>
    /// Mark that this unit has moved this turn.
    /// </summary>
    public void SetHasMovedThisTurn()
    {
        HasMovedThisTurn = true;
    }

    /// <summary>
    /// Mark that this unit has attacked this turn, and dim the visual so we know.
    /// </summary>
    public void SetHasAttackedThisTurn()
    {
        HasAttackedThisTurn = true;

        // Darken the visual.
        Color c = Visual.GetComponent<SpriteRenderer>().color;
        Visual.GetComponent<SpriteRenderer>().color = new Color(
            c.r - HasMovedDarkenAmount,
            c.g - HasMovedDarkenAmount,
            c.b - HasMovedDarkenAmount,
            c.a);
    }

    /// <summary>
    /// Take damage and check if I died.
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Perform any actions on this unit that should happen when a new turn starts.
    /// </summary>
    public void OnTurnEnd()
    {
        // When the turn ends, reset the movement variables.
        SetCanMoveAndActAgain();
    }

    /// <summary>
    /// After undoing movement or advancing the turn, reset the movement variables so this character can move again.
    /// </summary>
    public void SetCanMoveAndActAgain()
    {
        HasMovedThisTurn = false;
        HasAttackedThisTurn = false;

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

    /// <summary>
    /// When a unit loses all of their health, this gets called. It should handle alerting subscribers that this unit has died.
    /// </summary>
    private void Die()
    {
        // Destroy the gameobject.
        GameObject.Destroy(gameObject);
    }
}
