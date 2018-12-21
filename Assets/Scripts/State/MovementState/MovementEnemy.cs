using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnemy : State
{
    Transform trsf;
    Vector3 targetPosition;

    GameObject player;
    Enemy enemy;

    Level level;

    Vector3 deltaPosition;

    //On peut suivre une position fixe ou en suivant un transform
    public MovementEnemy(Character chara, Level level, Transform trsf) : base(chara)
    {
        this.trsf = trsf;
        this.level = level;
        enemy = character.GetComponent<Enemy>();
    }

    public override void EndState()
    {
        //Fin de l'etat, on a plus besoin de connaitre les triggers
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
        if (coll.gameObject.tag == "FollowParent")
        {
            character.SetState(new MovementEnemy(character, level, coll.transform.parent));
        }
    }

    public override void UpdateState()
    {
        deltaPosition = trsf.position - character.transform.position;

        // If the enemies have reached the player, they enter the Movement Attack phase
        if (Vector3.Distance(trsf.position, character.transform.position) <= Mathf.Abs(character.transform.position.y - trsf.position.y) + enemy.range)
        {
            character.SetState(new EnemyAttack(character, level, player));
        }

        character.Move(deltaPosition.normalized);
    }

    void Separate(List<GameObject> characters)
    {
        float desiredseparation = 3f;
        float maxForce = 2f;
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
                Vector3.ClampMagnitude(steer, maxForce);
                character.GetComponent<Rigidbody>().AddForce(steer);
            }
        }
    }
}
