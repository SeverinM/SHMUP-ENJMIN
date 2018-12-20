using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnemy : State
{
    public enum MovementState {
        START,
        NORMAL,
        ESCAPE,
        LOOP,
        RANDOM_LOOP
    }

    Transform trsf;
    Vector3 targetPosition;
    Queue<Vector3> positions;
    GameObject player;
    Enemy enemy;
    List<GameObject> characters;

    MovementState state;

    //On peut suivre une position fixe ou en suivant un transform
    public MovementEnemy(Character chara, List<GameObject> characters, GameObject player, Transform trsf, MovementState state) : base(chara)
    {
        this.trsf = trsf;
        this.player = player;
        enemy = character.GetComponent<Enemy>();
        this.characters = characters;
    }

    // Random movement position
    public MovementEnemy(Character chara, List<GameObject> characters, GameObject player, MovementState state) : base(chara)
    {
        this.player = player;
        enemy = character.GetComponent<Enemy>();
        this.characters = characters;
    }

    public MovementEnemy(Character chara, List<GameObject> characters, GameObject player, Queue<Vector3> allPos, MovementState state) : base(chara)
    {
        positions = allPos;
        this.state = state;
        this.player = player;
        this.characters = characters;
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
                character.SetState(new MovementEnemy(character, characters, player, coll.transform.parent, MovementState.NORMAL));
            }
        }

    }

    public override void UpdateState()
    {
        Vector3 deltaPosition;

        Separate();

        if (trsf != null)
        {
            deltaPosition = trsf.position - character.transform.position;

            // If the enemies have reached the player, they enter the Movement Attack phase
            if (Vector3.Distance(trsf.position,character.transform.position) <= Mathf.Abs(character.transform.position.y - trsf.position.y) + enemy.range)
            {
                character.SetState(new EnemyAttack(character, characters, player));
            }
        }
        else
        {
            deltaPosition = targetPosition - character.transform.position;
            
            if (Vector3.Distance(targetPosition, character.transform.position) < 0.1f)
            {
                if (positions.Count > 0)
                {
                    character.SetState(new MovementEnemy(character, characters, player, positions, MovementState.NORMAL));
                }
                else  // After Start movement state, the enemies seek the player
                {
                    character.SetState(new MovementEnemy(character, characters, player, player.transform, MovementState.NORMAL));
                }
            }
        }
      
        character.Move(deltaPosition.normalized);
    }

    void Separate()
    {
        float desiredseparation = 3;
        Vector3 sum = new Vector3();
        int count = 0;

        if (characters != null)
        {
            foreach (GameObject other in characters)
            {
                float d = Vector3.Distance(character.transform.position, other.transform.position);
                if ((d > 0) && (d < desiredseparation))
                {
                    Vector3 diff = character.transform.position - other.transform.position;
                    diff.Normalize();
                    diff /= d;
                    sum += diff;
                    count++;

                }
            }

            if (count > 0)
            {
                sum /= count;
                sum.Normalize();
                sum *= 5f;
                Vector3 steer = sum - character.GetComponent<Rigidbody>().velocity;
                // steer.limit(maxforce);
                character.GetComponent<Rigidbody>().AddForce(steer);
            }
        }
    }
}
