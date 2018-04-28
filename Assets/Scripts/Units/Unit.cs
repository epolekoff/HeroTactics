using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour{

    /// <summary>
    /// The sprite representing this character.
    /// </summary>
    public GameObject Visual;

    /// <summary>
    /// Stats for this character.
    /// </summary>
    public UnitStats Stats;

    /// <summary>
    /// The position of this unit on the map.
    /// </summary>
    public Vector3 TilePosition;

    /// <summary>
    /// Is this unit an enemy?
    /// </summary>
    public virtual bool IsEnemy { get; set; }

    // Update is called once per frame
    void Update()
    {
        // Keep the sprite billboarded.
        BillboardHeroSprite();
    }

    /// <summary>
    /// Keep the sprite facing the camera.
    /// </summary>
    private void BillboardHeroSprite()
    {
        // Look at the camera
        //Visual.transform.LookAt(Camera.main.transform);

        // Keep the sprites Isometric
        Visual.transform.rotation = Camera.main.transform.rotation;
    }
}
