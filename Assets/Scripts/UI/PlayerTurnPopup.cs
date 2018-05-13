using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerTurnPopup : MonoBehaviour {

    public Text PlayerNameText;
    public Image TextBackground;
    public Outline TextOutline;

    private readonly Dictionary<System.Type, string> PlayerStrings = new Dictionary<System.Type, string>()
    {
        { typeof(HumanPlayer), "Heroes' Turn" },
        { typeof(EnemyPlayer), "Enemies' Turn" }
    };

    private readonly Dictionary<System.Type, Color> PlayerColors = new Dictionary<System.Type, Color>()
    {
        { typeof(HumanPlayer), new Color(0.3f, 0, 0.6875f) },
        { typeof(EnemyPlayer), Color.red }
    };

    /// <summary>
    /// Trigger the popup.
    /// </summary>
    public void TriggerPopup(Player player)
    {
        // Set the name on the popup.
        string popupText = PlayerStrings[player.GetType()];
        PlayerNameText.text = popupText;

        // Set the colors
        TextOutline.effectColor = PlayerColors[player.GetType()];
        TextBackground.color = PlayerColors[player.GetType()];

        // Trigger the popup animation
        GetComponent<Animator>().SetTrigger("Popup");
    }
}
