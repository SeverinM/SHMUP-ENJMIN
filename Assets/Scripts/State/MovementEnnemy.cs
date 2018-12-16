using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnnemy : State
{
    public enum MovementState {
        START,
        NORMAL,
        ESCAPE,
        LOOP
    }

    Transform trsf;
    Vector3 targetPosition;
    Queue<Vector3> positions;

    MovementState state;

    //On peut suivre une position fixe ou en suivant un transform
    public MovementEnnemy(Character chara, Transform trsf, MovementState state) : base(chara)
    {
        this.trsf = trsf;
    }

    public MovementEnnemy(Character chara, Queue<Vector3> allPos, MovementState state) : base(chara)
    {
        positions = allPos;
        this.state = state;
        Vector3 vecInput = positions.Dequeue();
        if (state == MovementState.LOOP)
        {
            positions.Enqueue(vecInput);
        }
        targetPosition = character.transform.position + vecInput;
        targetPosition = new Vector3(targetPosition.x, character.transform.position.y , targetPosition.z);
    }

    public override void EndState()
    {
        //Fin de l'etat ,on a plus besoin de connaitre les triggers
        character.OnTriggerEnterChar -= TriggerEnter;
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void StartState()
    {
        //On veut savoir si le personnage touche un certain trigger
        character.OnTriggerEnterChar += TriggerEnter;
    }

    public void TriggerEnter(Collider coll)
    {

        if (!state.Equals(MovementState.START)) // If enemy is not in start phase, then you can follow the player
        {
            if (coll.gameObject.tag == "FollowParent")
            {
                character.SetState(new MovementEnnemy(character, coll.transform.parent, MovementState.NORMAL));
            }
        }

    }

    public override void UpdateState()
    {
        Vector3 deltaPosition;

        if (trsf != null)
        {
            deltaPosition = trsf.position - character.transform.position;

            // If the enemies have reached the player, they enter the Movement Attack phase
            if (Vector3.Distance(trsf.position,character.transform.position) <= Mathf.Abs(character.transform.position.y - trsf.position.y) + 0.01f)
            {
                character.SetState(new EnemyAttack(character));
            }
        }
        else
        {
            deltaPosition = targetPosition - character.transform.position;
            
            if (Vector3.Distance(targetPosition, character.transform.position) < 0.1f)
            {
               
                if (positions.Count > 0)
                {
                    character.SetState(new MovementEnnemy(character, positions, MovementState.LOOP));
                }
                else  // After Start movement state, the enemies seek the player
                {
                    character.SetState(null);
                }
            }
        }
      
        character.Move(deltaPosition.normalized);
    }
}
