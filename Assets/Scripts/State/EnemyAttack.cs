using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chaque ennemi tire sur le joueur selon une période de temps
/// </summary>
public class EnemyAttack : State
{
    private Enemy enemy;

    private float lastTime = 0;
    private float shoots = 0;
    public float totalTime = 0;
    Queue<WaypointElement> elements;
    Transform playerTarget;

    public EnemyAttack(Character character, Queue<WaypointElement> elt, Transform player) : base(character)
    {
        enemy = character.GetComponent<Enemy>();
        elements = elt;
        playerTarget = player;
    }
    
    public override void EndState()
    {
        base.EndState();
        character.Context.Remove("Target");
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        //Apres avoir attaqué , revient en mode aleatoire
        if (character.Context.ValuesOrDefault<Transform>("FollowButAvoid",null) != null)
        {
            enemy.FollowRandomPath();
            return;
        }

        //Il existe un leader , il continue a le suivre
        if (character.Leader != null)
        {
            character.SetState(new EnemyMovement(character, character.Leader.transform, elements, true));
        }
        else
        {
            character.SetState(new EnemyMovement(character, playerTarget, elements, false));
        }
    }

    public override void StartState()
    {
        base.StartState();
        character.Context.SetInDictionary("Target", playerTarget);
    }

    public override void UpdateState()
    {
        if (playerTarget == null)
        {
            enemy.FollowRandomPath();
            return;
        }
        character.transform.forward = Vector3.RotateTowards(character.transform.forward,(playerTarget.position - character.transform.position).normalized, (1 / character.CoeffRotation) * Mathf.Deg2Rad, 0.0f);
        character.transform.forward = new Vector3(character.transform.forward.x, 0, character.transform.forward.z);
        // Lance une attaque selon la periode
        totalTime += Time.deltaTime * character.PersonalScale * character.GetScale();
        if (totalTime > lastTime)
        {
            lastTime += enemy.ShootPeriod;
            character.transform.LookAt(playerTarget);

            if (!character.Context.ValuesOrDefault<bool>("InRecovery",false))
                enemy.Shoot();

            shoots++;

            if (shoots == enemy.ShootAmount)
            {
                NextState();
            }
        }
    }

    public override string GetName()
    {
        return "EnemyAttack";
    }
}
