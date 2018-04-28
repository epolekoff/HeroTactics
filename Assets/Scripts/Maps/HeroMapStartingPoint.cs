using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class HeroMapStartingPoint {

    [SerializeField()]
    public HeroType Hero;

    [SerializeField()]
    public Vector3 Position;
}

[System.Serializable()]
public class EnemyMapStartingPoint
{
    [SerializeField()]
    public EnemyType Enemy;

    [SerializeField()]
    public Vector3 Position;
}