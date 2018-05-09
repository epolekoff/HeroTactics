using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitStatsPanel : MonoBehaviour {

    /// <summary>
    /// Health
    /// </summary>
    public Text HealthText;

    /// <summary>
    /// Undo the selected unit's movement.
    /// </summary>
    public Button UndoMovementButton;


	/// <summary>
    /// Start
    /// </summary>
	void Start ()
    {
		
	}
	
	/// <summary>
    /// Update
    /// </summary>
	void Update ()
    {
        // Get the selected unit
        Unit selectedUnit = GameManager.Instance.HumanPlayer.SelectedUnit;
        if (selectedUnit == null)
        {
            return;
        }

        // Update the health text.
        SetHealthText(selectedUnit.CurrentHealth, selectedUnit.Stats.MaxHealth);

        // Show or hide the undo movement button depending on whether or not we can undo.
    }

    /// <summary>
    /// Set the health text based on the health values.
    /// </summary>
    private void SetHealthText(int health, int maxHealth)
    {
        HealthText.text = string.Format("Health: {0}/{1}", health, maxHealth);
    }

    /// <summary>
    /// Callback for the undo movement button.
    /// </summary>
    public void OnUndoMovementButtonPressed()
    {
        // Get the human player and undo their movement.
        GameManager.Instance.HumanPlayer.UndoMovement();
    }
}
