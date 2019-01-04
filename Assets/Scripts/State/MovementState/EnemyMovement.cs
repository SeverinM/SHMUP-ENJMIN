﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permet à un joueur de se déplacer vers une Transform (un obstacle, un enemy ou le joueur)
/// </summary>
public class EnemyMovement : State
{
    Vector3 targetPosition;
    Enemy enemy;
    Transform target;
    Vector3 deltaPosition;
    Queue<WaypointElement> allElems;
    bool followLeader;

    public EnemyMovement(Character chara, Transform tar, Queue<WaypointElement> elt, bool followLeader = false) : base(chara)
    {
        allElems = elt;
        enemy = (Enemy)character;
        target = tar;
        this.followLeader = followLeader;
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void UpdateState()
    {
        // Si la cible à été détruit, alors on se déplace aléatoirement
        if (target == null)
        {
            enemy.FollowRandomPath();
            return;
        }

        deltaPosition = target.transform.position - character.transform.position;

        character.Separate();

        if (!followLeader)
        {
            character.transform.LookAt(target.transform);
            // L'ennemie est proche du joueur
            if (Vector3.Distance(target.transform.position, character.transform.position) <= enemy.AttackRange)
            {
                character.SetState(new EnemyAttack(character, allElems, target.transform));
            }
        }
        //La vitesse du personnage est de plus en plus lente au fur et a mesure qu'il s'approche de son leader pour eviter de lui rentrer dedans
        else
        {
            character.PersonalScale = Mathf.Clamp(Vector3.Distance(target.transform.position, character.transform.position) / enemy.MoveSpeed, 0, 1);
            character.transform.forward = deltaPosition;
        }

        character.Move(deltaPosition.normalized);
    }

    public override void StartState()
    {
        character.OnTriggerExitChar += TriggerExit;
    }

    public override void EndState()
    {
        character.OnTriggerExitChar -= TriggerExit;
    }

    public void TriggerExit(Collider coll)
    {
        //Quitter la zone du joueur n'a de sens que si l'ennemi poursuivait le joueur
        if (!followLeader && coll.tag == "FollowParent")
        {
            character.SetState(new FollowPathMovement(character, allElems, ((Enemy)character).Waypoints.loop));
        }
    }

}
