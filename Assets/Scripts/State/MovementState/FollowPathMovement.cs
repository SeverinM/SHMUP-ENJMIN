using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    Enemy enn;

    public FollowPathMovement(Character character, Queue<WaypointElement> allPos, bool loop) : base(character)
    {
        positions = allPos;
        enn = (Enemy)character;

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

        this.loop = loop;
    }

    public override void StartState()
    {
        if (targetPosition - character.transform.position != Vector3.zero)
            character.transform.forward = targetPosition - character.transform.position;

        character.OnTriggerEnterChar += TriggerEnter;
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
        deltaPosition = targetPosition - character.transform.position;

        //Waypoint atteint ?
        if (Vector3.Distance(targetPosition, character.transform.position) < (GetSpeed() * character.PersonalScale * character.GetScale() * Time.deltaTime * 4))
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
                if (loop && (Enemy)character != null)
                {
                    character.SetState(new FollowPathMovement(character,new Queue<WaypointElement>(((Enemy)character).Waypoints.allWaypoints), ((Enemy)character).Waypoints.loop));
                }
                else
                {
                    enn.FollowRandomPath();
                }
            }    
        }

        if (deltaPosition != Vector3.zero)
            character.transform.forward = deltaPosition;

        character.Move(deltaPosition.normalized);
    }

    float GetSpeed()
    {
        if (currentWaypoint != null)
        {
            return currentWaypoint.speed;
        }
        else
        {
            return 1;
        }
    }

    public override string GetName()
    {
        return "FollowPathMovement";
    }
}
