using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : Singleton<GameManager>, IStateMachineEntity
{
    /// <summary>
    /// The camera.
    /// </summary>
    public GameCamera GameCamera;

    /// <summary>
    /// A mapping of legal unit ids to their prefabs.
    /// </summary>
    public HeroRegistry HeroRegistry;
    public EnemyRegistry EnemyRegistry;

    /// <summary>
    /// For demo purposes, where to the heroes each start.
    /// </summary>
    public List<HeroMapStartingPoint> DemoStartingPoints;
    public List<EnemyMapStartingPoint> DemoEnemyStartingPoints;

    /// <summary>
    /// Get access to the map.
    /// </summary>
    public GameMap Map { get { return m_map; } }

    /// <summary>
    /// Get access to the heroes.
    /// </summary>
    public List<Hero> Heroes { get { return m_heroes; } }

    /// <summary>
    /// The players in the game (probably only 1).
    /// </summary>
    public Player Player;

    private List<Hero> m_heroes;
    private List<Enemy> m_enemies;
    private GameMap m_map;

    private FiniteStateMachine m_stateMachine;
    public FiniteStateMachine GetStateMachine(int number = 0) { return m_stateMachine; }

    /// <summary>
    /// Initialization
    /// </summary>
    void Awake ()
    {
        // Create the player object.
        Player = new Player();

        // Generate the map
        MapFileData mapData = MapFactory.ReadMap("TestLevelHeight");
        m_map = MapFactory.GenerateMap(mapData);

        // Generate the heroes in the registry
        m_heroes = HeroFactory.CreateAllHeroesAtTestStartingPoints(HeroRegistry, DemoStartingPoints, m_map);

        // Generate the enemies in the registry
        m_enemies = EnemyFactory.CreateAllEnemiesAtTestStartingPoints(EnemyRegistry, DemoEnemyStartingPoints, m_map);

        // Set up the state machine
        m_stateMachine = new FiniteStateMachine(new PlayerTurnState(), this);
    }
	
	/// <summary>
    /// Update
    /// </summary>
	void Update ()
    {
        m_stateMachine.Update();
    }
}
