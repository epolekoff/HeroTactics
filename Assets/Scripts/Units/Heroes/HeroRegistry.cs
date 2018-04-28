using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroRegistry", menuName = "Hero/Registry", order = 100)]
public class HeroRegistry : ScriptableObject {

    [SerializeField()]
    public List<HeroRegistryEntry> AllHeroes;
}

[System.Serializable()]
public class HeroRegistryEntry
{
    /// <summary>
    /// The hero representing this prefab.
    /// </summary>
    [SerializeField()]
    public HeroType HeroType;

    /// <summary>
    /// Where the prefab for this hero is located.
    /// </summary>
    [SerializeField()]
    public GameObject HeroPrefab;
}