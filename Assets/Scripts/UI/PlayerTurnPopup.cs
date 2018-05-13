using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerTurnPopup : MonoBehaviour {

    public Text PlayerNameText;

    private readonly Dictionary<System.Type, string> PlayerStrings = new Dictionary<System.Type, string>()
    {
        { typeof(HumanPlayer), "Heroes' Turn" },
        { typeof(EnemyPlayer), "Enemies' Turn" }
    };

    /// <summary>
    /// Trigger the popup.
    /// </summary>
    public void TriggerPopup(Player player)
    {
        // Set the name on the popup.
        string popupText = PlayerStrings[player.GetType()];
        PlayerNameText.text = popupText;

        // Trigger the popup animation
        GetComponent<Animator>().SetTrigger("Popup");
    }
}
