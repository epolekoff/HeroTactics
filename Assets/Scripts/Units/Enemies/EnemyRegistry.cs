using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyRegistry", menuName = "Enemy/Registry", order = 100)]
public class EnemyRegistry : ScriptableObject {

    [SerializeField()]
    public List<EnemyRegistryEntry> AllEnemies;
}

[Serializable]
public class EnemyRegistryEntry
{
    /// <summary>
    /// The enemy type.
    /// </summary>
    [SerializeField()]
    public EnemyType EnemyType;

    /// <summary>
    /// The prefab to use for this enemy.
    /// </summary>
    [SerializeField()]
    public GameObject EnemyPrefab;
}
