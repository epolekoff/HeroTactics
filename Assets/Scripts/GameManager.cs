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

    /// <summary>
    /// Get access to the map.
    /// </summary>
    public GameMap Map { get { return m_map; } }

    /// <summary>
    /// Get access to the heroes.
    /// </summary>
    public List<Hero> Heroes { get { return m_heroes; } }


    private List<Hero> m_heroes;
    private GameMap m_map;

	/// <summary>
    /// Initialization
    /// </summary>
	void Awake ()
    {
        // Generate the map
        MapFileData mapData = MapFactory.ReadMap("TestLevelHeight");
        m_map = MapFactory.GenerateMap(mapData);

        // Generate the heroes in the registry
        m_heroes = HeroFactory.CreateAllHeroesAtTestStartingPoints(HeroRegistry, DemoStartingPoints, m_map);
    }
	
	/// <summary>
    /// Update
    /// </summary>
	void Update ()
    {

    }
}
