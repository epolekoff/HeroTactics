using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	// Use this for initialization
	void Start ()
    {

        // Generate the map
        Dictionary<Vector2, int> mapHeights = MapFactory.ReadMap("TestLevelHeight");
        MapFactory.GenerateMap(mapHeights);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
