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

    public GenerateEnemies(Character character, List<Wave> remainingWaves) : base(character)
    {
        //On enleve la vague du haut
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
        currentWave.allEnnemies.ForEach(x =>
        {
            if (x.spawnAfter < timeSinceBegin && !x.spawned)
            {
                GameObject instanciated = level.Instanciate(x.enn, character.transform.position);
                instanciated.GetComponent<Enemy>().enemyType = x.enn;
                instanciated.GetComponent<Enemy>().SetWaypointsAndApply(x.Waypoints);
                x.spawned = true;
            }
        });

        //Plus d'ennemi a spawn , on peut passer a la vague suivante
        if (currentWave.allEnnemies.Where(x => !x.spawned).Count() == 0)
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
    }
}
