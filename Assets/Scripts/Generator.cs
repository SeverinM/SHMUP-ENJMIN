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
    public int Count = 0;

    public delegate void noParam();
    public event noParam EveryoneDied;

    //Combien d'ennemie reste t'il 
    public int EnnemiesLeftToSpawn
    {
        get
        {
            return allWaves.Select(x => x.allEnnemies).Count() + Count;
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
        return Constants.TimeScaleGenerators;
    }

    public void RaiseEveryoneDied()
    {
        if (EveryoneDied != null)
        {
            EveryoneDied();
        }
    }
}
