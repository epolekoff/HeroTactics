using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Hero/HeroStats", order = 1)]
public class HeroStats : ScriptableObject {

    /// <summary>
    /// The name to display for this hero.
    /// </summary>
    [SerializeField()]
    public string DisplayName;

    /// <summary>
    /// Health of this hero.
    /// </summary>
    [SerializeField()]
    public int MaxHealth;

    /// <summary>
    /// Movement range for this hero.
    /// </summary>
    [SerializeField()]
    public int MovementRange;
}
