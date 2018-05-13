using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour {

    public UnitStatsPanel UnitStatsPanel;
    public PlayerTurnPopup PlayerTurnPopup;

    /// <summary>
    /// When starting the game, initialize some of the UI elements as hidden, in case they are left active while developing.
    /// </summary>
    void Start()
    {
        UnitStatsPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the panel.
    /// </summary>
    public void ShowUnitStatsPanel(bool show)
    {
        UnitStatsPanel.gameObject.SetActive(show);
    }

    /// <summary>
    /// Show the panel.
    /// </summary>
    public void TriggerPlayerTurnPopup(Player player)
    {
        PlayerTurnPopup.TriggerPopup(player);
    }
}
