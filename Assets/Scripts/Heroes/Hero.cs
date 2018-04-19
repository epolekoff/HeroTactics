﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    /// <summary>
    /// The hero that this object represents.
    /// </summary>
    public HeroId HeroId;

    /// <summary>
    /// Stats for this character.
    /// </summary>
    public HeroStats Stats;

    /// <summary>
    /// The sprite representing this character.
    /// </summary>
    public GameObject Visual;

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
        Visual.transform.LookAt(Camera.main.transform);
    }
}
