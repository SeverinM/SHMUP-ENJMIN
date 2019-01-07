using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LockWaveElement
{
    public Generator generator;
    public int number;

    public override bool Equals(object obj)
    {
        LockWaveElement elt = (LockWaveElement)obj;
        if (elt == null)
        {
            return false;
        }
        else
        {
            return (generator == elt.generator && number == elt.number);
        }
    }

    public override int GetHashCode()
    {
        return generator.GetHashCode() * number.GetHashCode();
    }
}

/// <summary>
/// The goal of this class is to keep the data of the Generator
/// </summary>
public class Generator : Character {

    public GameObject levelObject;        // Level
    [HideInInspector]
    public List<Wave> allWaves = new List<Wave>();
    public int Count = 0;
    public int WaveCount = 0;
    public Dictionary<int, List<LockWaveElement>> AllLocks = new Dictionary<int, List<LockWaveElement>>();

    public delegate void noParam();
    public event noParam EveryoneDied;

    public delegate void paramClean(int nb, Generator who);
    public event paramClean WaveCleaned;

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

    public void RaiseWaveFinished()
    {
        WaveCleaned(WaveCount, this);
    }

    public void RemoveLock(LockWaveElement elt)
    {
        foreach(List<LockWaveElement> eltList in AllLocks.Values)
        {
            eltList.RemoveAll(x => x == elt);
        }
    }
}
