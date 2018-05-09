using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum HeroType
{
    None,
    Luffy,
}

public static class HeroFactory
{
    /// <summary>
    /// Create all of the specified heroes at their starting points.
    /// </summary>
    /// <param name="heroRegistry"></param>
    /// <returns></returns>
    public static List<Unit> CreateAllHeroesAtTestStartingPoints(HeroRegistry heroRegistry, List<HeroMapStartingPoint> startingPoints, GameMap map)
    {
        List<Unit> newHeroes = new List<Unit>();

        foreach(var startingPoint in startingPoints)
        {
            Hero newHero = CreateHero(heroRegistry, startingPoint.Hero);
            newHeroes.Add(newHero);

            // Position the heroes at their starting points.
            map.MoveObjectToTile(newHero, startingPoint.Position, true);
        }

        return newHeroes;
    }

    /// <summary>
    /// Create a hero object. Requires a registry holding the matching prefab.
    /// </summary>
    /// <returns></returns>
	public static Hero CreateHero(HeroRegistry heroRegistry, HeroType id)
    {
        var heroObject = GameObject.Instantiate(heroRegistry.AllHeroes.FirstOrDefault(h => h.HeroType == id).HeroPrefab);
        if(heroObject == null || heroObject.GetComponent<Hero>() == null)
        {
            Debug.LogError("Unable to load hero " + id.ToString());
            return null;
        }

        // Set up the hero class.
        Hero hero = heroObject.GetComponent<Hero>();
        hero.Initialize();
        hero.HeroType = id;

        return hero;
    }
}
