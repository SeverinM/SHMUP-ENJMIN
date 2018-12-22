using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Permet à un personnage de suivre un chemin , un etat correspond a un seul chemin , pour en faire plusieurs il faut donc repasser dans cet etat
/// </summary>
public class FollowPathMovement : State
{
    Queue<Vector3> positions;
    Vector3 targetPosition;

    private Vector3 deltaPosition;

    private Level level;

    private bool loop;

    public FollowPathMovement(Character character, Level level, Queue<Vector3> allPos, bool loop, float noiseCoeff = 0) : base(character)
    {
        positions = allPos;
        //Position relative a l'ennemi
        Vector3 vecInput;
        if (positions.Count > 0)
        {
            vecInput = positions.Dequeue();
            //La position revient en debut de queue
            if (loop)
            {
                positions.Enqueue(vecInput);
            }
        }
        else
        {
            vecInput = Vector3.zero;
        }

        Vector3 randomValue;
        if (level != null)
        {
            randomValue = new Vector3(Random.Range(level.minBounds.x, level.maxBounds.y), 0, Random.Range(level.maxBounds.x, level.maxBounds.y));
        }
        else
        {
            randomValue = new Vector3(Random.Range(-2,2), 0, Random.Range(-2, 2));
        }
        
        vecInput += (randomValue * noiseCoeff);
        targetPosition = character.transform.position + vecInput;

        //On l'aligne sur l'axe y par rapport au joueur 
        targetPosition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);
        this.loop = loop;
        this.level = level;
        character.transform.forward = vecInput;
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
            character.SetState(new EnemyMovement(character, level, coll.transform.parent));
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
                character.SetState(new FollowPathMovement(character, level, positions, loop, 0));
            }
            else 
            {
                character.SetState(new FollowPathMovement(character, level, positions, loop, 1));
            }
        }
        character.Move(deltaPosition.normalized * character.GetScale());
    }
}
