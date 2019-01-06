﻿using UnityEngine;
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

    GameObject leader;

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
            generator.RaiseEveryoneDied();
            character.SetState(null);
        }
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        timeSinceBegin += Time.deltaTime * character.GetScale();
        if (currentWave.firstIsLeader)
        {
            WaveElement x = currentWave.allEnnemies.First();
            if (x != null && x.spawnAfter < timeSinceBegin && !x.spawned)
            {
                leader = level.AddEnemy(x.enn, character.transform.position);
                Enemy enn = leader.GetComponent<Enemy>();
                enn.enemyType = x.enn;
                enn.movementType = x.enMov;
                enn.SetWaypointsAndApply(x.Waypoints);
                enn.Destroyed += EnnemyDestroyed;
                x.spawned = true;
                currentWave.firstIsLeader = false;
            }
        }
        
        // Pour chaque ennemi de la vague courrante
        currentWave.allEnnemies.ForEach(x =>
        {
            // Si l'enemi courrant doit spawn et n'a pas encore spawné
            if (x.spawnAfter < timeSinceBegin && !x.spawned)
            {
                generator.Count++;
                GameObject instanciated = level.AddEnemy(x.enn, character.transform.position);
                Enemy enn = instanciated.GetComponent<Enemy>();
                enn.enemyType = x.enn;
                enn.movementType = x.enMov;
                enn.SetWaypointsAndApply(x.Waypoints); // On attribue les waypoints
                enn.Leader = x.followPlayer ? level.Player : leader;
                enn.Destroyed += EnnemyDestroyed; // Quand le joueur est détruit, il notifie GenerateEnemies
                x.spawned = true;
            }
        });
    }

    void EnnemyDestroyed(Character chara)
    {
        count--;
        generator.Count--;
        // Si tous les énnemis on été détruits, on passe à l'état suivant
        if (count == 0)
        {
            NextState();
        }
    }
}
