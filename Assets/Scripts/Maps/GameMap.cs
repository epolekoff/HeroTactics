using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour {

    private const float ValidNeighborHeightDifference = 1;
    private const int MaxMapHeight = 10;

    public int Width;
    public int Depth;

    public GameObject[,,] ObjectsOnTiles;
    public Dictionary<Vector3, MapTile> MapTiles = new Dictionary<Vector3, MapTile>();

    private const float ShooterLerpTimePerTimeTraveled = 0.1f;

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

	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Move an object to a new tile. Record where it is now.
    /// </summary>
    public void MoveObjectToTile(Unit unit, Vector3 tilePosition, bool firstTimeSetup = false, System.Action callback = null)
    {
        // Error checking
        if(!MapTiles.ContainsKey(tilePosition))
        {
            Debug.LogError(string.Format("Attempting to move unit {3} to invalid tile ({0}, {1}, {2}).", tilePosition.x, tilePosition.y, tilePosition.z, unit.name));
            return;
        }

        // Get the shooter's old position
        Vector3 oldPosition = unit.TilePosition;

        // Move the object to the new position.
        ObjectsOnTiles[(int)tilePosition.x, (int)tilePosition.y, (int)tilePosition.z] = unit.gameObject;
        unit.TilePosition = tilePosition;

        int distanceTraveled = (int)Mathf.Abs(oldPosition.x - (int)tilePosition.x) + (int)Mathf.Abs(oldPosition.z - (int)tilePosition.z);

        // Move the game object
        Vector3 desiredPosition = new Vector3(
            tilePosition.x * MapTile.Width,
            MapTile.Height * MapTiles[unit.TilePosition].Position.y,
            tilePosition.z * MapTile.Width);
        if (firstTimeSetup)
        {
            unit.transform.position = desiredPosition;
        }
        else
        {
            StartCoroutine(LerpObjectToPosition(unit.transform, desiredPosition, distanceTraveled, callback));
        }

        // Delete the object from the old position
        if (!firstTimeSetup)
        {
            ObjectsOnTiles[(int)oldPosition.x, (int)oldPosition.y, (int)oldPosition.z] = null;
        }
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
    /// Get the neighbors of this tile.
    /// </summary>
    public List<MapTile> GetValidNeighbors(Vector3 tilePosition)
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

        List<MapTile> neighbors = new List<MapTile>();

        // Same elevation
        Vector3 left = new Vector3(x - 1, y, z);
        Vector3 right = new Vector3(x + 1, y, z);
        Vector3 front = new Vector3(x, y, z + 1);
        Vector3 back = new Vector3(x, y, z - 1);

        // Up in elevation
        //Vector3 up = tilePosition + new Vector3(0, 1, 0);
        Vector3 up_left = left + new Vector3(0, 1, 0);
        Vector3 up_right = right + new Vector3(0, 1, 0);
        Vector3 up_front = front + new Vector3(0, 1, 0);
        Vector3 up_back = back + new Vector3(0, 1, 0);

        // Down in elevation
        //Vector3 down = tilePosition - new Vector3(0, 1, 0);
        Vector3 down_left = left - new Vector3(0, 1, 0);
        Vector3 down_right = right - new Vector3(0, 1, 0);
        Vector3 down_front = front - new Vector3(0, 1, 0);
        Vector3 down_back = back - new Vector3(0, 1, 0);


        if (IsValidTilePosition(left) && Mathf.Abs(left.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[left]);
        if (IsValidTilePosition(right) && Mathf.Abs(right.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[right]);
        if (IsValidTilePosition(front) && Mathf.Abs(front.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[front]);
        if (IsValidTilePosition(back) && Mathf.Abs(back.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[back]);

        if (IsValidTilePosition(up_left) && Mathf.Abs(up_left.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[up_left]);
        if (IsValidTilePosition(up_right) && Mathf.Abs(up_right.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[up_right]);
        if (IsValidTilePosition(up_front) && Mathf.Abs(up_front.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[up_front]);
        if (IsValidTilePosition(up_back) && Mathf.Abs(up_back.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[up_back]);

        if (IsValidTilePosition(down_left) && Mathf.Abs(down_left.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[down_left]);
        if (IsValidTilePosition(down_right) && Mathf.Abs(down_right.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[down_right]);
        if (IsValidTilePosition(down_front) && Mathf.Abs(down_front.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[down_front]);
        if (IsValidTilePosition(down_back) && Mathf.Abs(down_back.z - tilePosition.z) <= ValidNeighborHeightDifference)
            neighbors.Add(MapTiles[down_back]);

        return neighbors;
    }

    /// <summary>
    /// Is it valid to move to this tile.
    /// </summary>
    public bool IsValidTilePosition(Vector3 tilePosition)
    {
        // Check against the map bounds.
        if (tilePosition.x < 0 || tilePosition.x >= Width ||
            tilePosition.y < 0 || tilePosition.y >= Depth ||
            tilePosition.z < 0 || tilePosition.z >= MaxMapHeight)
        {
            return false;
        }

        // The tile needs to exist.
        if(!MapTiles.ContainsKey(tilePosition))
        {
            return false;
        }

        // Check other unit objects on the tile.
        if (GetUnitOnTile(tilePosition))
        {
            return false;
        }

        // If a tile is directly above this tile, it cannot be stood on.
        if(MapTiles.ContainsKey(tilePosition + Vector3.up))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Recurse through all neighbors and get a list of tiles in range, without duplicates.
    /// </summary>
    public List<MapTile> GetAllTilesInRange(Vector3 tile, int range)
    {
        // Get all tiles, filter out duplicates.
        HashSet<MapTile> tilesInRangeNoDuplicates = new HashSet<MapTile>();
        List<MapTile> allTilesInRange = GetAllTilesInRangeRecursive(tile, 0, range);
        foreach (var potentialTile in allTilesInRange)
        {
            tilesInRangeNoDuplicates.Add(potentialTile);
        }

        return new List<MapTile>(tilesInRangeNoDuplicates);
    }

    private List<MapTile> GetAllTilesInRangeRecursive(Vector3 tilePosition, int depth, int maxDepth)
    {
        if (depth == maxDepth)
        {
            List<MapTile> list = new List<MapTile>();
            if (IsValidTilePosition(tilePosition))
            {
                list.Add(MapTiles[tilePosition]);
            }
            return list;
        }

        // Keep recursing.
        List<MapTile> totalList = new List<MapTile>();
        if (IsValidTilePosition(tilePosition))
        {
            totalList.Add(MapTiles[tilePosition]);
        }
        foreach (var neighbor in GetValidNeighbors(tilePosition))
        {
            totalList.AddRange(GetAllTilesInRangeRecursive(neighbor.Position, depth + 1, maxDepth));
        }
        return totalList;
    }

    /// <summary>
    /// Highlight the tiles that indicate this player's movement range.
    /// </summary>
    public void HighlightMovementRange(Unit unit)
    {
        ClearHighlightedTiles();

        HighlightState highlight = unit.IsEnemy ? HighlightState.Enemy : HighlightState.Friendly;

        List<MapTile> allTilesInRange = GetAllTilesInRange(unit.TilePosition, unit.Stats.MovementRange);
        foreach (MapTile tile in allTilesInRange)
        {
            tile.ShowHighlight(highlight);
            m_highlightedTiles.Add(tile);
        }
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
    /// Smoothely move shooter to the correct time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpObjectToPosition(Transform transform, Vector3 desiredPosition, int distanceTraveled, System.Action callback)
    {
        float lerpTime = distanceTraveled * ShooterLerpTimePerTimeTraveled;
        Vector3 startPosition = transform.position;
        float timer = 0;

        while (timer < lerpTime)
        {
            timer += Time.deltaTime;
            var ratio = timer / lerpTime;

            transform.position = Vector3.Lerp(startPosition, desiredPosition, ratio);

            yield return new WaitForEndOfFrame();
        }

        if (callback != null)
        {
            callback();
        }
    }
}
