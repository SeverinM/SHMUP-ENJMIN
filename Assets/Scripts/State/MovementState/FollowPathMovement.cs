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
        character.Context.Remove("Target");
        positions = allPos;
        if (allPos.Count > 0)
        {
            currentWaypoint = allPos.Peek();
            targetPosition = currentWaypoint.targetPosition;
        }
        else
        {
            targetPosition = character.transform.position + new Vector3(1, 0, 1);
        }

        Vector3 randomValue = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));

        targetPosition += (randomValue * noiseCoeff);

        //On l'aligne sur l'axe y par rapport au joueur 
        targetPosition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);
        this.loop = loop;
        character.transform.forward = targetPosition - character.transform.position;
    }

    public override void StartState()
    {
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
            character.Context.SetInDictionary("Target", coll.transform.parent);
            character.SetState(new EnemyMovement(character, coll.transform.parent,positions));
        }

        if (coll.tag == "Hook")
        {
            character.SetState(new FreezeMovement(character, character.ActualState));
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

        if (Vector3.Distance(targetPosition, character.transform.position) < 0.1f)
        {
            if (positions.Count > 0)
            {
                //Le mouvement est consideré comme accompli , on le retire
                positions.Dequeue();
                character.SetState(new FollowPathMovement(character, positions, loop, 0));
            }
            else
            {
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
        character.Move(deltaPosition.normalized * (currentWaypoint != null ? currentWaypoint.speed : 1));
    }
}
