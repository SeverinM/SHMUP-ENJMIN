using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The goal of this class is to keep the data of the Generator
/// </summary>
public class Generator : Character {

    public GameObject levelObject;        // Level
    [HideInInspector]
    public List<Wave> allWaves = new List<Wave>();

    public delegate void noParam();
    public event noParam EveryoneDied;

    //Combien d'ennemie reste t'il a spawn ?
    public int EnnemiesLeftToSpawn
    {
        get
        {
            return allWaves.Select(x => x.allEnnemies).Count();
        }
    }

    public List<Wave> AllWaves
    {
        get
        {
            return allWaves;
        }
        set
        {
            allWaves = value;
        }
    }

    public override float GetScale()
    {
        return 1;
    }

    public void RaiseEveryoneDied()
    {
        if (EveryoneDied != null)
        {
            EveryoneDied();
        }
    }
}
