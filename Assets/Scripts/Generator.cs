using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The goal of this class is to keep the data 
/// </summary>
public class Generator : Character {

    public GameObject enemyPrefab;        // Instantiable enemy
    public GameObject levelObject;        // Level
    public int maxEnemies = 16;           // Max ennemies on the scene
    public int waves = 4;                 // Waves
    public float period = 1f;             // Period of time to generate enemy
    public float radiusSize = 4f;         // Period of time to generate enemy


}
