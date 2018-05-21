using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum EnemyType
{
    None,
    ShortRange,
    LongRange
}

public static class EnemyFactory
{

    /// <summary>
    /// Create all of the specified enemies at their starting points.
    /// </summary>
    /// <returns></returns>
    public static List<Unit> CreateAllEnemiesAtTestStartingPoints(EnemyRegistry registry, List<EnemyMapStartingPoint> startingPoints, GameMap map)
    {
        List<Unit> newEnemies = new List<Unit>();

        foreach (var startingPoint in startingPoints)
        {
            Enemy newEnemy = CreateEnemy(registry, startingPoint.Enemy);
            newEnemies.Add(newEnemy);

            // Position the enemies at their starting points.
            map.MoveObjectToTile(newEnemy, startingPoint.Position, forceImmediate: true);
        }

        return newEnemies;
    }

    /// <summary>
    /// Create an enemy object. Requires a registry holding the matching prefab.
    /// </summary>
    /// <returns></returns>
	public static Enemy CreateEnemy(EnemyRegistry registry, EnemyType type)
    {
        var enemyObject = GameObject.Instantiate(registry.AllEnemies.FirstOrDefault(h => h.EnemyType == type).EnemyPrefab);
        if (enemyObject == null || enemyObject.GetComponent<Enemy>() == null)
        {
            Debug.LogError("Unable to load enemy " + type.ToString());
            return null;
        }

        // Set up the enemy class.
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.EnemyType = type;
        enemy.Initialize();

        return enemy;
    }
}
