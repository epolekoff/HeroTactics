﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

/// <summary>
/// Determines what tiles should be filtered out of a search.
/// This allows movement through allies, attacks only on enemies, etc.
/// </summary>
public class MapTileFilterInfo
{
    /// <summary>
    /// Allow tiles with these units to appear in the filter.
    /// Useful to allow units to pass through allies.
    /// </summary>
    public bool AlliesOk;
    public bool EnemiesOk;

    /// <summary>
    /// Require units of these types to appear on the tile.
    /// Useful for casting healing, or attacking enemies.
    /// </summary>
    public bool AlliesRequired;
    public bool EnemiesRequired;

    /// <summary>
    /// Disallow a tile with any unit on it from appearing.
    /// This is the last step, so you can pass through allies, but not stop on them.
    /// </summary>
    public bool NoStoppingOnAllies;
    public bool NoStoppingOnEnemies;

    /// <summary>
    /// The height difference allowed when getting neighbors.
    /// </summary>
    public int HeightDifferenceAllowed = 1;

    /// <summary>
    /// The player doing the filtering. This allows the algorithm to know who is an Ally or an Enemy.
    /// </summary>
    public Player Player;
}

public class GameMap : MonoBehaviour
{
    private const int MaxMapHeight = 10;

    public int Width;
    public int Depth;

    public GameObject[,,] ObjectsOnTiles;
    public Dictionary<Vector3, MapTile> MapTiles = new Dictionary<Vector3, MapTile>();

    private const float UnitLerpTimePerTileTraveled = 0.1f;

    private List<MapTile> m_highlightedTiles = new List<MapTile>();


    /// <summary>
    /// Setup from the factory.
    /// </summary>
    public void Intialize(int width, int depth)
    {
        Width = width;
        Depth = depth;

        ObjectsOnTiles = new GameObject[width, MaxMapHeight, depth];
    }

    /// <summary>
    /// Move an object to a new tile. Record where it is now.
    /// </summary>
    public void MoveObjectToTile(Unit unit, Vector3 tilePosition, bool forceImmediate = false, System.Action callback = null)
    {
        // Error checking
        if(!MapTiles.ContainsKey(tilePosition))
        {
            Debug.LogError(string.Format("Attempting to move unit {3} to invalid tile ({0}, {1}, {2}).", tilePosition.x, tilePosition.y, tilePosition.z, unit.name));
            return;
        }

        // Move the game object
        if (forceImmediate)
        {
            unit.transform.position = GetWorldSpacePositionOfMapTilePosition(tilePosition); 
            if(callback != null)
            {
                callback();
            }
        }
        else
        {
            // Get the path of travel
            MapTile start = MapTiles[unit.TilePosition];
            MapTile goal = MapTiles[tilePosition];
            MapTileFilterInfo tileFilterInfo = new MapTileFilterInfo() { AlliesOk = true, NoStoppingOnAllies = true, NoStoppingOnEnemies = true, Player = unit.Owner };
            List<MapTile> path = Pathfinder.GetPath(GameManager.Instance.Map, start, goal, tileFilterInfo);

            StartCoroutine(LerpObjectAlongPath(unit, path, callback));
        }

        // Get the shooter's old position
        Vector3 oldPosition = unit.TilePosition;
        unit.PreviousTilePosition = oldPosition;

        // Move the object to the new position.
        ObjectsOnTiles[(int)tilePosition.x, (int)tilePosition.y, (int)tilePosition.z] = unit.gameObject;
        unit.TilePosition = tilePosition;

        // Delete the object from the old position
        if (IsTilePositionInBounds(oldPosition))
        {
            ObjectsOnTiles[(int)oldPosition.x, (int)oldPosition.y, (int)oldPosition.z] = null;
        }
    }

    /// <summary>
    /// Given a map tile position, get its world space position.
    /// </summary>
    public Vector3 GetWorldSpacePositionOfMapTilePosition(Vector3 mapTilePosition)
    {
        return new Vector3(
            mapTilePosition.x * MapTile.Width,
            MapTile.Height * MapTiles[mapTilePosition].Position.y,
            mapTilePosition.z * MapTile.Width);
    }

    /// <summary>
    /// Get the unit on a tile, or null if there is none.
    /// </summary>
    public Unit GetUnitOnTile(Vector3 tile)
    {
        GameObject objectOnTile = ObjectsOnTiles[(int)tile.x, (int)tile.y, (int)tile.z];

        if (objectOnTile == null)
        {
            return null;
        }

        Unit unit = objectOnTile.GetComponent<Unit>();
        return unit;
    }

    /// <summary>
    /// Remove a shooter from a tile.
    /// </summary>
    public void RemoveObjectFromTiles(Unit shooter)
    {
        ObjectsOnTiles[(int)shooter.TilePosition.x, (int)shooter.TilePosition.y, (int)shooter.TilePosition.z] = null;
    }

    /// <summary>
    /// Highlight the tiles that indicate this player's movement range.
    /// </summary>
    public void HighlightMovementRange(Unit unit)
    {
        ClearHighlightedTiles();

        HighlightState highlight = unit.Owner is EnemyPlayer ? HighlightState.Enemy : HighlightState.Friendly;

        MapTileFilterInfo tileFilterInfo = new MapTileFilterInfo()
        {
            AlliesOk = true,
            NoStoppingOnAllies = true,
            NoStoppingOnEnemies = true,
            Player = unit.Owner,
        };

        List<MapTile> allTilesInRange = GetAllTilesInRange(unit.TilePosition, unit.Stats.MovementRange, tileFilterInfo);
        foreach (MapTile tile in allTilesInRange)
        {
            tile.ShowHighlight(highlight);
            m_highlightedTiles.Add(tile);
        }
    }

    /// <summary>
    /// Get all tiles in range of a tile and highlight them as actionable.
    /// </summary>
    public void HighlightActionRange(Vector3 startTile, UnitActionRange range, int rangeValue, MapTileFilterInfo tileFilterInfo)
    {
        ClearHighlightedTiles();

        List<MapTile> mapTilesInRange = GetAllMapTilesInActionRange(startTile, range, rangeValue, tileFilterInfo);
        foreach(var mapTile in mapTilesInRange)
        {
            m_highlightedTiles.Add(mapTile);
            mapTile.ShowHighlight(HighlightState.Attack);
        }
    }

    /// <summary>
    /// Given an action range, return a list of tiles that fall in that range of the original tile.
    /// </summary>
    /// <returns></returns>
    public List<MapTile> GetAllMapTilesInActionRange(Vector3 startTile, UnitActionRange range, int rangeValue, MapTileFilterInfo tileFilterInfo)
    {
        List<MapTile> mapTilesInActionRange = new List<MapTile>();

        switch(range)
        {
            case UnitActionRange.Adjacent:
                mapTilesInActionRange = GetAllTilesInRange(startTile, rangeValue, tileFilterInfo);
                break;
            case UnitActionRange.Self:
                mapTilesInActionRange.Add(MapTiles[startTile]);
                break;
            case UnitActionRange.SkipOneTile:
                List<MapTile> adjacentMapTiles = GetAllTilesInRange(startTile, 1, tileFilterInfo);
                List<MapTile> rangeOfTwo = GetAllTilesInRange(startTile, rangeValue, tileFilterInfo);
                mapTilesInActionRange = rangeOfTwo.Except(adjacentMapTiles).ToList();
                break;
        }

        return mapTilesInActionRange;
    }

    /// <summary>
    /// Hide the highlight on all tiles that have highlights.
    /// </summary>
    public void ClearHighlightedTiles()
    {
        foreach (var mapTile in m_highlightedTiles)
        {
            mapTile.ShowHighlight(HighlightState.None);
        }

        m_highlightedTiles.Clear();
    }

    /// <summary>
    /// Get the neighbors of this tile.
    /// </summary>
    public List<MapTile> GetValidNeighbors(Vector3 tilePosition, MapTileFilterInfo tileFilterInfo)
    {
        int x = (int)tilePosition.x;
        int y = (int)tilePosition.y;
        int z = (int)tilePosition.z;

        // If this tile itself does not exist, then we cannot get its neighbors.
        if(!MapTiles.ContainsKey(tilePosition))
        {
            return new List<MapTile>();
        }

        MapTile mapTile = MapTiles[tilePosition];

        // Iterate all of my neighbors at every height difference.
        List<MapTile> neighbors = new List<MapTile>();
        for (int neighborY = y - tileFilterInfo.HeightDifferenceAllowed; neighborY <= y + tileFilterInfo.HeightDifferenceAllowed; neighborY++)
        {
            Vector3 left = new Vector3(x - 1, neighborY, z);
            Vector3 right = new Vector3(x + 1, neighborY, z);
            Vector3 front = new Vector3(x, neighborY, z + 1);
            Vector3 back = new Vector3(x, neighborY, z - 1);

            if (IsValidTilePosition(left, tileFilterInfo) && Mathf.Abs(left.y - tilePosition.y) <= tileFilterInfo.HeightDifferenceAllowed)
                neighbors.Add(MapTiles[left]);
            if (IsValidTilePosition(right, tileFilterInfo) && Mathf.Abs(right.y - tilePosition.y) <= tileFilterInfo.HeightDifferenceAllowed)
                neighbors.Add(MapTiles[right]);
            if (IsValidTilePosition(front, tileFilterInfo) && Mathf.Abs(front.y - tilePosition.y) <= tileFilterInfo.HeightDifferenceAllowed)
                neighbors.Add(MapTiles[front]);
            if (IsValidTilePosition(back, tileFilterInfo) && Mathf.Abs(back.y - tilePosition.y) <= tileFilterInfo.HeightDifferenceAllowed)
                neighbors.Add(MapTiles[back]);
        }

        return neighbors;
    }

    /// <summary>
    /// Check if the tile position is within bounds
    /// </summary>
    public bool IsTilePositionInBounds(Vector3 tilePosition)
    {
        if (tilePosition.x < 0 || tilePosition.x >= Width ||
            tilePosition.y < 0 || tilePosition.y >= MaxMapHeight ||
            tilePosition.z < 0 || tilePosition.z >= Depth)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Is it valid to move to this tile.
    /// </summary>
    public bool IsValidTilePosition(Vector3 tilePosition, MapTileFilterInfo tileFilterInfo)
    {
        // Check against the map bounds.
        if(!IsTilePositionInBounds(tilePosition))
        {
            return false;
        }

        // The tile needs to exist.
        if(!MapTiles.ContainsKey(tilePosition))
        {
            return false;
        }

        // Check other unit objects on the tile.
        // Sometimes, we want to move through allies or only attack enemies.
        Unit unitOnTile = GetUnitOnTile(tilePosition);
        if (unitOnTile != null)
        {
            if (unitOnTile.IsEnemyOf(tileFilterInfo.Player) && (tileFilterInfo.AlliesRequired || !tileFilterInfo.EnemiesOk))
            {
                return false;
            }
            if(!unitOnTile.IsEnemyOf(tileFilterInfo.Player) && (tileFilterInfo.EnemiesRequired || !tileFilterInfo.AlliesOk))
            {
                return false;
            }
        }
        else
        {
            // If a tile is directly above this tile, it cannot be stood on.
            if (MapTiles.ContainsKey(tilePosition + Vector3.up))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Recurse through all neighbors and get a list of tiles in range, without duplicates.
    /// </summary>
    public List<MapTile> GetAllTilesInRange(Vector3 tile, int range, MapTileFilterInfo tileFilterInfo)
    {
        // Get all tiles, filter out duplicates.
        HashSet<MapTile> tilesInRangeNoDuplicates = new HashSet<MapTile>();
        List<MapTile> allTilesInRange = GetAllTilesInRangeRecursive(tile, tileFilterInfo, 0, range);
        foreach (var potentialTile in allTilesInRange)
        {
            // If the filtering info says we can't stand on the same tile as another unit, then remove those tiles.
            if (!CanStopOnThisTile(potentialTile.Position, tileFilterInfo))
            {
                continue;
            }

            // Remove duplicate tiles
            tilesInRangeNoDuplicates.Add(potentialTile);
        }
        
        return new List<MapTile>(tilesInRangeNoDuplicates);
    }

    public List<MapTile> GetPath(MapTile start, MapTile goal, MapTileFilterInfo tileFilterInfo, int maxMoveRange = -1)
    {
        // Use the pathfinder to get a path.
        var path = Pathfinder.GetPath(this, start, goal, tileFilterInfo);

        // If the move range is too short, trim the path down.
        if (maxMoveRange > 0 && maxMoveRange < path.Count)
        {
            path.RemoveRange(maxMoveRange, path.Count - maxMoveRange);
        }
        if (path.Count == 0)
        {
            return path;
        }

        // Step backwards from the end to remove any invalid landing spaces. This prevents the path from ending on a unit, potentially.
        bool lastTileInPathIsValid = false;
        while (!lastTileInPathIsValid && path.Count > 0)
        {
            // If the filtering info says we can't stand on the same tile as another unit, then remove those tiles.
            lastTileInPathIsValid = CanStopOnThisTile(path[path.Count - 1].Position, tileFilterInfo);

            // Remove the last tile since it is invalid.
            if(!lastTileInPathIsValid)
            {
                path.RemoveAt(path.Count - 1);
            }
        }

        return path;
            
    }

    /// <summary>
    /// Check if the filter info allows a unit to stop on the specified tile.
    /// </summary>
    public bool CanStopOnThisTile(Vector3 tilePosition, MapTileFilterInfo tileFilterInfo)
    {
        Unit unitOnTile = GetUnitOnTile(tilePosition);
        if (unitOnTile != null &&
            ((tileFilterInfo.NoStoppingOnAllies && !unitOnTile.IsEnemyOf(tileFilterInfo.Player)) ||
            (tileFilterInfo.NoStoppingOnEnemies && unitOnTile.IsEnemyOf(tileFilterInfo.Player))))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Given a tile, create a new tile on top of it.
    /// </summary>
    public void CreateMapTileOnTopOfTile(MapTile existingTile)
    {
        if(GetUnitOnTile(existingTile.Position) != null)
        {
            Debug.LogError(string.Format("Attempting to create tile on top of existing tile that has a unit on it. Position {0}", existingTile.Position));
            return;
        }

        Vector3 newTilePosition = new Vector3(existingTile.Position.x, existingTile.Position.y + 1, existingTile.Position.z);
        MapTile newTile = MapFactory.GenerateMapTile(newTilePosition);
        newTile.RegisterToMap(this);
    }

    /// <summary>
    /// Recursive function to get all tiles in a specified range.
    /// </summary>
    /// <returns></returns>
    private List<MapTile> GetAllTilesInRangeRecursive(Vector3 tilePosition, MapTileFilterInfo tileFilterInfo, int depth, int maxDepth)
    {
        if (depth == maxDepth)
        {
            List<MapTile> list = new List<MapTile>();
            if (IsValidTilePosition(tilePosition, tileFilterInfo))
            {
                list.Add(MapTiles[tilePosition]);
            }
            return list;
        }

        // Keep recursing.
        List<MapTile> totalList = new List<MapTile>();
        if (IsValidTilePosition(tilePosition, tileFilterInfo))
        {
            totalList.Add(MapTiles[tilePosition]);
        }
        foreach (var neighbor in GetValidNeighbors(tilePosition, tileFilterInfo))
        {
            totalList.AddRange(GetAllTilesInRangeRecursive(neighbor.Position, tileFilterInfo, depth + 1, maxDepth));
        }
        return totalList;
    }

    /// <summary>
    /// Smoothely move unit to the correct tile through a path.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpObjectAlongPath(Unit unit, List<MapTile> path, System.Action callback)
    {
        Transform transform = unit.transform;

        unit.IsMoving = true;

        for(int i = 0; i < path.Count; i++)
        {
            float timer = 0;
            Vector3 startPosition = transform.position;
            Vector3 desiredPosition = GetWorldSpacePositionOfMapTilePosition(path[i].Position);
            while (timer < UnitLerpTimePerTileTraveled)
            {
                timer += Time.deltaTime;
                var ratio = timer / UnitLerpTimePerTileTraveled;

                transform.position = Vector3.Lerp(startPosition, desiredPosition, ratio);

                yield return new WaitForEndOfFrame();
            }
        }
        unit.IsMoving = false;
        if (callback != null)
        {
            callback();
        }
    }
}
