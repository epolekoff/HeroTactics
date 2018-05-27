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
    /// The action buttons.
    /// </summary>
    public List<Button> ActionButtons;

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
        SetActionButtonText(selectedUnit.AvailableActions);

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
    /// Set the text on each of the buttons to say what move it is.
    /// </summary>
    /// <param name="unitActions"></param>
    private void SetActionButtonText(List<UnitAction> unitActions)
    {
        // Iterate all buttons that we have.
        for(int i = 0; i < ActionButtons.Count; i++)
        {
            // Assign the name if the action exists.
            if(i < unitActions.Count)
            {
                ActionButtons[i].GetComponentInChildren<Text>().text = unitActions[i].Name;
            }
        }
    }

    /// <summary>
    /// Callback for the undo movement button.
    /// </summary>
    public void OnUndoMovementButtonPressed()
    {
        // Get the human player and undo their movement.
        GameManager.Instance.HumanPlayer.UndoMovement();
    }

    /// <summary>
    /// Select an action for the selected unit.
    /// </summary>
    public void OnActionButtonClicked(int index)
    {
        GameManager.Instance.HumanPlayer.SelectAction(index);
    }

    /// <summary>
    /// End my turn.
    /// </summary>
    public void OnEndTurnButtonPressed()
    {
        GameManager.Instance.HumanPlayer.EndTurn();
    }
}
