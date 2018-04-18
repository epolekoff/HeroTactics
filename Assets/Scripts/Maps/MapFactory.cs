using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class MapFactory {

    private const string MapTextAssetPath = "Maps/";

    private const string MapTileObjectReference = "Map/MapTile";
    private const string MapTileDirtReference = "Map/MapTileDirt";
    private const string MapTileGrassReference = "Map/MapTileGrass";
    private const float MapTileHeight = 0.5f;
    private const float MapTileWidth = 1f;

    /// <summary>
    /// Hacky way to generate a map for the prototype.
    /// Read the map file and generate a number of tiles based on the height at each position.
    /// </summary>
    /// <param name="filename"></param>
	public static Dictionary<Vector2, int> ReadMap(string filename)
    {
        TextAsset mapTextAsset = Resources.Load(MapTextAssetPath + filename) as TextAsset;

        // Iterate the lines of the map and place them into a 2D array.
        Dictionary<Vector2, int> mapHeights = new Dictionary<Vector2, int>();
        int y = 0;
        foreach (var line in mapTextAsset.text.Split(new[] { '\n', '\r'}))
        {
            if(line.Length == 0 ||  line.Contains("#"))
            {
                continue;
            }

            List<int> lineHeights = new List<int>();
            int x = 0;
            string lineWithoutSpaces = line.Replace(" ", string.Empty);
            foreach(var character in lineWithoutSpaces)
            {
                int height = int.Parse(character.ToString());
                lineHeights.Add(height);
                mapHeights.Add(new Vector2(x, y), height);
                x++;
            }
            y++;
        }

        return mapHeights;
    }

    /// <summary>
    /// Generate all of the tiles and align them properly.
    /// </summary>
    /// <param name="mapHeights"></param>
    /// <returns></returns>
    public static GameObject GenerateMap(Dictionary<Vector2, int> mapHeights)
    {
        GameObject mapObject = new GameObject("Map");
        foreach(var kvp in mapHeights)
        {
            GenerateMapTile(kvp.Key, kvp.Value, mapObject);
        }

        return mapObject;
    }

    /// <summary>
    /// Instantiate a map tile at the correct position.
    /// </summary>
    private static void GenerateMapTile(Vector2 tile, int height, GameObject mapParent)
    {
        var mapTileResource = Resources.Load(MapTileObjectReference);
        var dirt = Resources.Load(MapTileDirtReference) as Material;
        var grass = Resources.Load(MapTileGrassReference) as Material;
        for (int i = 0; i < height; i++)
        {
            var mapTileObject = GameObject.Instantiate(
                mapTileResource,
                new Vector3(tile.x * MapTileWidth, i * MapTileHeight, tile.y * MapTileWidth),
                Quaternion.identity,
                mapParent.transform) as GameObject;

            // Set the tile as dirt or grass.
            if(i == height - 1)
            {
                mapTileObject.GetComponentInChildren<Renderer>().material = grass;
            }
            else
            {
                mapTileObject.GetComponentInChildren<Renderer>().material = dirt;
            }
        }
    }
}
