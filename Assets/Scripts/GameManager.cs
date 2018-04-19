using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Singleton<GameManager> {

    /// <summary>
    /// A mapping of legal hero ids to their prefabs.
    /// </summary>
    public HeroRegistry HeroRegistry;

    /// <summary>
    /// For demo purposes, where to the heroes each start.
    /// </summary>
    public List<HeroMapStartingPoint> DemoStartingPoints;

    private List<Hero> m_heroes;

	// Use this for initialization
	void Start ()
    {

        // Generate the map
        Dictionary<Vector2, int> mapHeights = MapFactory.ReadMap("TestLevelHeight");
        MapFactory.GenerateMap(mapHeights);

        // Generate the heroes in the registry
        m_heroes = HeroFactory.CreateAllHeroesAtTestStartingPoints(HeroRegistry, DemoStartingPoints, mapHeights);

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
