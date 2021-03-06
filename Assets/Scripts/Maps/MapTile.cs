﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HighlightState
{
    None,
    Friendly,
    Enemy,
    Attack,
    TargetEnemy
}

public class MapTile : MonoBehaviour {


    // All tiles are squares, and the same size.
    public static float Width = 1f;

    // Tiles are scaled slightly smaller height-wise so they look nice.
    public static float Height = 1f;

    /// <summary>
    /// The highlight object.
    /// </summary>
    public HighlightState HighlightState { get; private set; }
    public GameObject HighlightFriendly;
    public GameObject HighlightEnemy;
    public GameObject HighlightAttack;

    /// <summary>
    /// The position of this tile in the grid.
    /// </summary>
    public Vector3 Position;


    /// <summary>
    /// Start
    /// </summary>
    void Start ()
    {
    }
	
	/// <summary>
    /// Update
    /// </summary>
	void Update () {
		
	}

    /// <summary>
    /// Equals just checks the tile position.
    /// </summary>
    public override bool Equals(object other)
    {
        MapTile otherTile = (MapTile)other;

        return Position == otherTile.Position;
    }

    /// <summary>
    /// Get Hash Code.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }

    /// <summary>
    /// When creating the map tiles, hook them up to be reverse engineered.
    /// </summary>
    public void RegisterToMap(GameMap gameMap)
    {
        // If the tile was already registered, remove it.
        if (gameMap.MapTiles.ContainsKey(Position))
        {
            GameObject.Destroy(gameMap.MapTiles[Position].gameObject);
            gameMap.MapTiles.Remove(Position);
        }

        // Set the parent
        transform.SetParent(gameMap.transform);

        // Register the new tile.
        gameMap.MapTiles.Add(Position, this);
    }

    /// <summary>
    /// Show the highlight, or hide it.
    /// </summary>
    public void ShowHighlight(HighlightState state)
    {
        HighlightState = state;
        switch (state)
        {
            case HighlightState.None:
                HighlightFriendly.SetActive(false);
                HighlightEnemy.SetActive(false);
                HighlightAttack.SetActive(false);
                break;
            case HighlightState.Friendly:
                HighlightFriendly.SetActive(true);
                HighlightEnemy.SetActive(false);
                HighlightAttack.SetActive(false);
                break;
            case HighlightState.Enemy:
                HighlightFriendly.SetActive(false);
                HighlightEnemy.SetActive(true);
                HighlightAttack.SetActive(false);
                break;
            case HighlightState.Attack:
                HighlightFriendly.SetActive(false);
                HighlightEnemy.SetActive(false);
                HighlightAttack.SetActive(true);
                break;
            case HighlightState.TargetEnemy:
                HighlightFriendly.SetActive(false);
                HighlightEnemy.SetActive(true);
                HighlightAttack.SetActive(false);
                break;
        }
    }
}
