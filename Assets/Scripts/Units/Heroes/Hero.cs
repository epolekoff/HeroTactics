using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    /// <summary>
    /// The hero that this object represents.
    /// </summary>
    public HeroId HeroId;

    /// <summary>
    /// Heroes are not enemies.
    /// </summary>
    public override bool IsEnemy {  get { return false; } }

	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update ()
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
