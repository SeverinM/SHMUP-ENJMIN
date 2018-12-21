﻿using UnityEngine;
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

    public FollowPathMovement(Character character, Level level, Queue<Vector3> allPos, bool loop) : base(character)
    {
        positions = allPos;
        Vector3 vecInput = positions.Dequeue();
        //Si on loop on replace à la fin de la queue
        if (loop)
        {
            positions.Enqueue(vecInput);
        }
        targetPosition = character.transform.position + vecInput;

        //On l'aligne sur l'axe y par rapport au joueur 
        targetPosition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);
        this.loop = loop;
        this.level = level;
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
                character.SetState(new FollowPathMovement(character, level, positions, loop));
            }
            else 
            {
                character.SetState(new WanderMovement(character, level));
            }
        }

        character.Move(deltaPosition.normalized);
    }
}
