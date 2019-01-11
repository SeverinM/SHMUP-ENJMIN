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

    // Permet au character d'être éloigné d'un certain espacement lorsqu'ils sont en file
    float spacingX;
    float spacingZ;

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
        generator.WaveCount++;
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
        timeSinceBegin += Time.deltaTime * character.GetScale() * character.PersonalScale;
        Vector3 leaderPos = character.transform.position;

        if (currentWave.firstIsLeader)
        {
            spacingX = currentWave.spacingX;
            spacingZ = currentWave.spacingZ;
            WaveElement x = currentWave.allEnnemies.First();
            if (x != null && x.spawnAfter < timeSinceBegin && !x.spawned)
            {
                leader = level.AddEnemy(x.enn, character.transform.position);
                Enemy enn = leader.GetComponent<Enemy>();
                enn.enemyType = x.enn;
                enn.movementType = x.enMov;
                enn.MoveSpeed = x.speed;
                enn.Life = x.life;
                enn.SetWaypointsAndApply(x.Waypoints);
                enn.Destroyed += EnnemyDestroyed;
                x.spawned = true;
                currentWave.firstIsLeader = false;
                if (x.followPlayer)
                    enn.SetState(new EnemyMovement(enn, level.Player.transform));
            }
        }
        

        foreach(WaveElement x in currentWave.allEnnemies)
        {
            // Si l'enemi courrant doit spawn et n'a pas encore spawné
            if (x.spawnAfter < timeSinceBegin && !x.spawned)
            {
                generator.Count++;

                leaderPos.x += spacingX;
                leaderPos.z += spacingZ;

                GameObject instanciated = level.AddEnemy(x.enn, leaderPos);
                Enemy enn = instanciated.GetComponent<Enemy>();
                enn.enemyType = x.enn;
                enn.movementType = x.enMov;
                if (enn.movementType == Enemy.EnemyMovementType.FOLLOW_PATH)
                {
                    enn.SetWaypointsAndApply(x.Waypoints);
                }
                // On attribue les waypoints
                enn.name = enn.name + " numero " + Generator.Number++;
                enn.Leader = x.followPlayer ? level.Player : leader;
                enn.Destroyed += EnnemyDestroyed; // Quand le joueur est détruit, il notifie GenerateEnemies
                enn.MoveSpeed = x.speed;
                enn.Life = x.life;
                x.spawned = true; //Cet ennemie ne peut pas etre spawn a nouveau
            }
        }
    }

    void EnnemyDestroyed(Character chara)
    {
        count--;
        generator.Count--;
        level.AddCharacterForScore(chara);
        // Si tous les énnemis on été détruits, on tente de passer a l'etat suivant
        if (count == 0)
        {
            generator.RaiseWaveFinished();
            generator.TryPassWave();
            if(wavesLeft.Count == 0)
            {
                generator.RaiseEveryoneDied();
            }
        }
    }

    public override string GetName()
    {
        return "GenerateEnemies";
    }
}
