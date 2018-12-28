using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The generators will Instanciate ennemies in the level according to a period
/// </summary>
public class GenerateEnemies : State
{
    private Generator generator;
    private Level level;
    float timeSinceBegin = 0;
    Wave currentWave;
    List<Wave> wavesLeft;

    public GenerateEnemies(Character character, List<Wave> remainingWaves) : base(character)
    {
        generator = character.GetComponent<Generator>();
        level = generator.levelObject.GetComponent<Level>();
        currentWave = remainingWaves[remainingWaves.Count - 1];
        remainingWaves.Remove(currentWave);
        wavesLeft = remainingWaves;
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        base.NextState();
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        timeSinceBegin += Time.deltaTime;
        foreach(WaveElement waveElt in currentWave.allEnnemies)
        {
            //Si le delai est depassé on crée l'ennemi et on le supprime
            if (waveElt.spawnAfter < timeSinceBegin)
            {
                GameObject instanciated = level.Instanciate(generator.enemyPrefab, character.transform.position);
                instanciated.GetComponent<Enemy>().enemyType = waveElt.enn;
                //!!!!
                currentWave.allEnnemies.Remove(waveElt);
            }
        }

        //Plus d'ennemi a spawn , on peut passer au suivant
        if (currentWave.allEnnemies.Count == 0 && wavesLeft.Count > 0)
        {
            character.SetState(new GenerateEnemies(character, wavesLeft));
        }
    }
}
