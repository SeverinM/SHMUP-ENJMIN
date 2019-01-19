using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Permet à un personnage de suivre un chemin , un etat correspond a un seul chemin , pour en faire plusieurs il faut donc repasser dans cet etat
/// </summary>
public class FollowPathMovement : State
{
    Queue<WaypointElement> positions;
    Vector3 targetPosition;
    WaypointElement currentWaypoint;
    private Vector3 deltaPosition;
    private bool loop;
    Vector3 beginAngle;
    float beginTime = 0;

    Action act;

    public FollowPathMovement(Character character, Queue<WaypointElement> allPos, bool loop) : base(character)
    {
        positions = allPos;

        //Si la queue n'est pas vide on prend la valeur cible sur le devant de la queue
        if (allPos.Count > 0)
        {
            currentWaypoint = allPos.Peek();
            targetPosition = currentWaypoint.targetPosition;
        }
        else
        {
            throw new System.Exception("Aucun waypoints d'attribué disponible");
        }

        float dist = Vector3.Distance(targetPosition, character.transform.position);
        this.loop = loop;
    }

    //Utilisé si l'on souhaite effectuer une action specifique a la fin des waypoints
    public FollowPathMovement(Character character,Queue<WaypointElement> allPos , Action after) : base(character)
    {
        positions = allPos;

        //Si la queue n'est pas vide on prend la valeur cible sur le devant de la queue
        if (allPos.Count > 0)
        {
            currentWaypoint = allPos.Peek();
            targetPosition = currentWaypoint.targetPosition;
        }
        else
        {
            throw new System.Exception("Aucun waypoints d'attribué disponible");
        }

        float dist = Vector3.Distance(targetPosition, character.transform.position);
        act = after;
        this.loop = false;
    }

    public override void StartState()
    {
        character.OnTriggerEnterChar += TriggerEnter;
        beginAngle = character.transform.forward;
    }

    public override void EndState()
    {
        character.OnTriggerEnterChar -= TriggerEnter;
    }

    public void TriggerEnter(Collider coll)
    {
        //L'ennemi est rentré dans la zone proche du joueur (seul les leader / ennemies seuls sont censés rentrer la dedans)
        if (coll.tag == "FollowParent" && character.Context.ValuesOrDefault<Transform>("FollowButAvoid", null) == null)
        {
            character.SetState(new EnemyMovement(character, coll.transform.parent, positions, false));
        }
    }


    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        base.NextState();
    }

    public override void UpdateState()
    {
        beginTime += (Time.deltaTime * character.GetScale() * character.PersonalScale * character.MoveSpeed * currentWaypoint.speed) / character.CoeffRotation;
        deltaPosition = targetPosition - character.transform.position;
        if (deltaPosition != Vector3.zero)
        {
            character.transform.forward = Vector3.Lerp(beginAngle, deltaPosition, beginTime);
        }

        float distanceToObjective = Vector3.Distance(targetPosition, character.transform.position);

        if (distanceToObjective < 0.3f)
        {
            positions.Dequeue();

            //Suit...mais pas trop
            Transform followButAvoid = character.Context.ValuesOrDefault<Transform>("FollowButAvoid", null);
            if (followButAvoid != null && character.GetScale() * character.PersonalScale > 0)
            {
                character.SetState(new EnemyAttack(character, positions, followButAvoid));
                return;
            }

            //Encore des waypoints a atteindre ?
            if (positions.Count > 0)
            {
                character.SetState(new FollowPathMovement(character, positions, loop));
            }
            else
            {
                //On recommence
                if (loop)
                {
                    character.SetState(new FollowPathMovement(character,new Queue<WaypointElement>(((Enemy)character).Waypoints.allWaypoints), ((Enemy)character).Waypoints.loop));
                }
                else
                {
                    if (act != null)
                        act();

                    if (character is Enemy)
                        ((Enemy)character).FollowRandomPath();
                }
            }    
        }
        float distanceBetweenFrame = Time.deltaTime * character.GetScale() * currentWaypoint.speed * character.MoveSpeed * character.PersonalScale;
        character.transform.position += deltaPosition.normalized * Mathf.Min(distanceToObjective, distanceBetweenFrame);
    }

    public override string GetName()
    {
        return "FollowPathMovement";
    }
}
