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

    public FollowPathMovement(Character character, Queue<WaypointElement> allPos, bool loop, float noiseCoeff = 0) : base(character)
    {
        positions = allPos;
        //Si la queue n'est pas vide on prend la valeur cible sur le devant de la queu
        if (allPos.Count > 0)
        {
            currentWaypoint = allPos.Peek();
            targetPosition = currentWaypoint.targetPosition;
        }
        //Dans le cas contraire la position cible et actuel sont la meme
        else
        {
            targetPosition = character.transform.position;
        }

        targetPosition += new Vector3(Random.Range(-noiseCoeff, noiseCoeff), 0, Random.Range(-noiseCoeff, noiseCoeff));
        this.loop = loop;
    }

    public override void StartState()
    {
        character.Context.Remove("Target");
        character.transform.forward = targetPosition - character.transform.position;
        character.OnTriggerEnterChar += TriggerEnter;
    }

    public override void EndState()
    {
        character.OnTriggerEnterChar -= TriggerEnter;
    }

    public void TriggerEnter(Collider coll)
    {
        //L'ennemi est rentré dans la zone proche du joueur , il va commencer a le poursuivre
        if (coll.tag == "FollowParent")
        {
            character.Context.SetInDictionary("Target", coll.transform);
            character.SetState(new EnemyMovement(character, coll.transform,positions));
        }

        if (coll.tag == "Hook")
        {
            character.PersonalScale = 0;
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
        if (Vector3.Distance(targetPosition, character.transform.position) < (GetSpeed() * Time.deltaTime * 4))
        {
            //Encore des waypoints a atteindre ?
            if (positions.Count > 0)
            {
                positions.Dequeue();
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
                    character.SetState(null);
                }
            }
            
        }
        character.transform.forward = deltaPosition;
        character.Move(deltaPosition.normalized * GetSpeed());
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
}
