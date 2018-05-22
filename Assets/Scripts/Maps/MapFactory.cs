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
    /// Instantiate a map tile at the correct position.
    /// </summary>
    private static List<MapTile> GenerateMapTile(Vector2 tile, int height, GameMap gameMap)
    {
        var mapTileResource = Resources.Load(MapTileObjectReference);
        var dirt = Resources.Load(MapTileDirtReference) as Material;
        var grass = Resources.Load(MapTileGrassReference) as Material;

        List<MapTile> mapTilesOnPoint = new List<MapTile>();
        for (int i = 0; i < height; i++)
        {
            float x = tile.x * MapTile.Width;
            float y = i * MapTile.Height;
            float z = tile.y * MapTile.Width;
            var mapTileObject = GameObject.Instantiate(
                mapTileResource,
                new Vector3(x, y, z),
                Quaternion.identity,
                gameMap.transform) as GameObject;

            // Set the tile as dirt or grass.
            if (i == height - 1)
            {
                mapTileObject.GetComponentInChildren<Renderer>().material = grass;
            }
            else
            {
                mapTileObject.GetComponentInChildren<Renderer>().material = dirt;
            }

            // Initialization
            MapTile newTile = mapTileObject.GetComponent<MapTile>();
            newTile.Position = new Vector3(tile.x, i, tile.y);
            mapTileObject.name = string.Format("MapTile({0},{1},{2})", (int)tile.x, (int)i, (int)tile.y);
            newTile.RegisterToMap(gameMap);

            mapTilesOnPoint.Add(newTile);
        }

        return mapTilesOnPoint;
    }
}
