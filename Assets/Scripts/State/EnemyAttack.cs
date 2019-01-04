using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chaque ennemi tire sur le joueur selon une période de temps
/// </summary>
public class EnemyAttack : State
{
    private Enemy enemy;

    private float lastTime;
    private float shoots = 0;
    Queue<WaypointElement> elements;
    Transform playerTarget;

    public EnemyAttack(Character character, Queue<WaypointElement> elt, Transform player) : base(character)
    {
        enemy = character.GetComponent<Enemy>();
        elements = elt;
        lastTime = Time.time;
        playerTarget = player;
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
        //Il existe un leader , il continue a le suivre
        if (character.Leader != null)
        {
            character.SetState(new EnemyMovement(character, character.Leader.transform, elements, true));
        }
        else
        {
            Debug.Log("fin");
            character.SetState(new EnemyMovement(character, playerTarget, elements, false));
        }
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        // Lance une attaque selon la periode
        if (lastTime < Time.time)
        {
            lastTime += enemy.ShootPeriod;
            character.transform.LookAt(playerTarget);

            enemy.Shoot();
            shoots++;

            if (shoots == enemy.ShootAmount)
            {
                NextState();
            }
        }
    }
}
