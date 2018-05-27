using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// Data read from the map file.
/// This may be placeholder until a better map system is in place.
/// </summary>
public struct MapFileData
{
    public Dictionary<Vector2, int> TileHeights;
    public int Width;
    public int Depth;
}

public static class MapFactory
{
    private const string MapTextAssetPath = "Maps/";

    private const string MapTileObjectReference = "Map/MapTile";
    private const string MapTileDirtReference = "Map/MapTileDirt";
    private const string MapTileGrassReference = "Map/MapTileGrass";

    /// <summary>
    /// Hacky way to generate a map for the prototype.
    /// Read the map file and generate a number of tiles based on the height at each position.
    /// </summary>
    /// <param name="filename"></param>
	public static MapFileData ReadMap(string filename)
    {
        TextAsset mapTextAsset = Resources.Load(MapTextAssetPath + filename) as TextAsset;

        // Iterate the lines of the map and place them into a 2D array.
        MapFileData mapData = new MapFileData();
        mapData.TileHeights = new Dictionary<Vector2, int>();
        int y = 0;
        int x = 0;
        foreach (var line in mapTextAsset.text.Split(new[] { '\n', '\r'}))
        {
            if(line.Length == 0 ||  line.Contains("#"))
            {
                continue;
            }

            List<int> lineHeights = new List<int>();
            x = 0;
            string lineWithoutSpaces = line.Replace(" ", string.Empty);
            foreach(var character in lineWithoutSpaces)
            {
                int height = int.Parse(character.ToString());
                lineHeights.Add(height);
                mapData.TileHeights.Add(new Vector2(x, y), height);
                x++;
            }
            y++;
        }

        mapData.Width = x;
        mapData.Depth = y;

        return mapData;
    }

    /// <summary>
    /// Generate all of the tiles and align them properly.
    /// </summary>
    /// <returns></returns>
    public static GameMap GenerateMap(MapFileData mapData)
    {
        // Create the map object.
        GameObject mapObject = new GameObject("Map");
        GameMap gameMap = mapObject.AddComponent<GameMap>();
        gameMap.Intialize(mapData.Width, mapData.Depth);

        // Create the tiles.
        foreach (var kvp in mapData.TileHeights)
        {
            GenerateMapTile(kvp.Key, kvp.Value, gameMap);
        }

        return gameMap;
    }

    /// <summary>
    /// Create a map tile in the scene.
    /// </summary>
    /// <returns></returns>
    public static MapTile GenerateMapTile(Vector3 position, GameObject mapTileResource = null)
    {
        // Load the resource if it was not provided.
        if (mapTileResource == null)
        {
            mapTileResource = Resources.Load(MapTileObjectReference) as GameObject;
        }

        // Create the object in the scene.
        float x = position.x * MapTile.Width;
        float y = position.y * MapTile.Height;
        float z = position.z * MapTile.Width;
        var mapTileObject = GameObject.Instantiate(
            mapTileResource,
            new Vector3(x, y, z),
            Quaternion.identity) as GameObject;

        // Initialization
        MapTile newTile = mapTileObject.GetComponent<MapTile>();
        newTile.Position = position;
        mapTileObject.name = string.Format("MapTile({0},{1},{2})", (int)position.x, (int)position.y, (int)position.z);

        return newTile;
    }

    /// <summary>
    /// Instantiate a map tile at the correct position.
    /// </summary>
    private static List<MapTile> GenerateMapTile(Vector2 tile, int height, GameMap gameMap)
    {
        GameObject mapTileResource = Resources.Load(MapTileObjectReference) as GameObject;
        var dirt = Resources.Load(MapTileDirtReference) as Material;
        var grass = Resources.Load(MapTileGrassReference) as Material;

        List<MapTile> mapTilesOnPoint = new List<MapTile>();
        for (int i = 0; i < height; i++)
        {
            MapTile newTile = GenerateMapTile(new Vector3(tile.x, i, tile.y), mapTileResource);

            // Set the tile as dirt or grass.
            if (i == height - 1)
            {
                newTile.GetComponentInChildren<Renderer>().material = grass;
            }
            else
            {
                newTile.GetComponentInChildren<Renderer>().material = dirt;
            }

            newTile.RegisterToMap(gameMap);

            mapTilesOnPoint.Add(newTile);
        }

        return mapTilesOnPoint;
    }
}
