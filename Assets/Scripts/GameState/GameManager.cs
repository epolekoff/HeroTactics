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
    /// The Canvas UI.
    /// </summary>
    public GameCanvas GameCanvas;

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
    /// The players in the game.
    /// </summary>
    public Player CurrentPlayer;
    public HumanPlayer HumanPlayer;
    public EnemyPlayer EnemyPlayer;

    private List<Unit> m_heroes;
    private List<Unit> m_enemies;
    private GameMap m_map;
    private bool m_currentPlayerIsHuman = true;

    private FiniteStateMachine m_stateMachine;
    public FiniteStateMachine GetStateMachine(int number = 0) { return m_stateMachine; }

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

        // Generate the enemies in the registry
        m_enemies = EnemyFactory.CreateAllEnemiesAtTestStartingPoints(EnemyRegistry, DemoEnemyStartingPoints, m_map);

        // Create the player objects.
        HumanPlayer = new HumanPlayer((List<Unit>)m_heroes);
        EnemyPlayer = new EnemyPlayer(m_enemies);
        CurrentPlayer = HumanPlayer;

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

    // End the current player's turn and transition to the next player's turn.
    public void EndCurrentPlayersTurn()
    {
        // Figure out who the new current player is.
        if(m_currentPlayerIsHuman)
        {
            CurrentPlayer = EnemyPlayer;
        }
        else
        {
            CurrentPlayer = HumanPlayer;
        }
        m_currentPlayerIsHuman = !m_currentPlayerIsHuman;

        // Tell the new player that it is their turn.
        CurrentPlayer.StartNewTurn();
    }
}
