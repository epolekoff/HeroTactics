using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For now, it's just 1 number, but it could be a particle system later.
/// </summary>
public class DamageNumber : MonoBehaviour
{
    /// <summary>
    /// The text that shows the number.
    /// </summary>
    public Text DamageText;



    /// <summary>
    /// Popup the damage number when taking damage.
    /// </summary>
    public void PopupDamageNumber(int damage)
    {
        DamageText.text = damage.ToString();
        GetComponent<Animator>().SetTrigger("Popup");
        BillboardText();
    }

    /// <summary>
    /// Keep the text facing the camera.
    /// </summary>
    private void BillboardText()
    {
        // Keep the sprites Isometric
        DamageText.transform.rotation = Camera.main.transform.rotation;
    }
}
