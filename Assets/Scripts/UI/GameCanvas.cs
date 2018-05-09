using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour {

    public UnitStatsPanel UnitStatsPanel;

    /// <summary>
    /// Show the panel.
    /// </summary>
    public void ShowUnitStatsPanel(bool show)
    {
        UnitStatsPanel.gameObject.SetActive(show);
    }
}
