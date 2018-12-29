using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The goal of this class is to keep the data of the Generator
/// </summary>
public class Generator : Character {

    public GameObject levelObject;        // Level
    [HideInInspector]
    public List<Wave> allWaves = new List<Wave>();

    public List<Wave> AllWaves
    {
        get
        {
            return allWaves;
        }
        //On relance le generation d'ennemi si cette valeur est modifié
        set
        {
            allWaves = value;
        }
    }

    public override float GetScale()
    {
        return 1;
    }
}
