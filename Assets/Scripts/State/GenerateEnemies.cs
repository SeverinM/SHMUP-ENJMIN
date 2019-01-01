using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    int count;

    public GenerateEnemies(Character character, List<Wave> remainingWaves) : base(character)
    {
        //On enleve la vague du haut
        generator = character.GetComponent<Generator>();
        level = generator.levelObject.GetComponent<Level>();
        currentWave = remainingWaves[remainingWaves.Count - 1];
        remainingWaves.Remove(currentWave);
        wavesLeft = remainingWaves;
        timeSinceBegin -= currentWave.delay;
        count = currentWave.allEnnemies.Count;
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
        if (wavesLeft.Count > 0)
        {
            character.SetState(new GenerateEnemies(character, wavesLeft));
        }
        else
        {
            character.SetState(null);
        }
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        timeSinceBegin += Time.deltaTime;
        currentWave.allEnnemies.ForEach(x =>
        {
            if (x.spawnAfter < timeSinceBegin && !x.spawned)
            {
                GameObject instanciated = level.Instanciate(x.enn, character.transform.position);
                Enemy enn = instanciated.GetComponent<Enemy>();
                enn.enemyType = x.enn;
                enn.GetComponent<Enemy>().SetWaypointsAndApply(x.Waypoints);
                enn.GetComponent<Enemy>().Destroyed += EnnemyDestroyed;
                x.spawned = true;
            }
        });
    }

    void EnnemyDestroyed(Character chara)
    {
        count--;
        if (count == 0)
        {
            NextState();
        }
    }
}
