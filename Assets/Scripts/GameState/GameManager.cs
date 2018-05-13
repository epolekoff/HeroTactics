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
        HumanPlayer = new HumanPlayer(m_heroes);
        EnemyPlayer = new EnemyPlayer(m_enemies);
        CurrentPlayer = HumanPlayer;

        // TODO: Only temporary. Show the turn UI popup
        GameCanvas.TriggerPlayerTurnPopup(CurrentPlayer);

        // Set up the state machine
        m_stateMachine = new FiniteStateMachine(new PlayerTurnState(), this);
    }
	
	/// <summary>
    /// Update
    /// </summary>
	void Update ()
    {
        m_stateMachine.Update();

        // Check all units and see if they are all dead yet.
        CheckGameHasEnded();
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

        // Show the turn UI popup
        GameCanvas.TriggerPlayerTurnPopup(CurrentPlayer);
    }

    /// <summary>
    /// Check all units to see if they are all dead.
    /// </summary>
    private void CheckGameHasEnded()
    {
        bool allPlayerUnitsDead = true;
        bool allEnemyUnitsDead = true;

        // Check player units.
        foreach (var unit in HumanPlayer.Units)
        {
            if(unit.CurrentHealth != 0)
            {
                allPlayerUnitsDead = false;
                break;
            }
        }
        if(allPlayerUnitsDead)
        {
            EndGame(EnemyPlayer);
        }

        // Check enemy units
        foreach (var unit in EnemyPlayer.Units)
        {
            if (unit.CurrentHealth != 0)
            {
                allEnemyUnitsDead = false;
                break;
            }
        }
        if (allEnemyUnitsDead)
        {
            EndGame(HumanPlayer);
        }
    }

    /// <summary>
    /// When a player wins, end the game and log who won.
    /// </summary>
    private void EndGame(Player winningPlayer)
    {
        Debug.Log(string.Format("Player {0} has won!", winningPlayer));
    }
}
