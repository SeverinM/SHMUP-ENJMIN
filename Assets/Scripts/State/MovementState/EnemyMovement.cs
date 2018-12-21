using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows a character to move towards a Transform (an obstacle, an enemy or the player)
/// </summary>
public class EnemyMovement : State
{
    Transform trsf;
    Vector3 targetPosition;

    GameObject player;
    Enemy enemy;

    Level level;

    Vector3 deltaPosition;

    public EnemyMovement(Character chara, Level level, Transform trsf) : base(chara)
    {
        this.trsf = trsf;
        this.level = level;
        enemy = character.GetComponent<Enemy>();
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void UpdateState()
    {
        deltaPosition = trsf.position - character.transform.position;

        // If the enemies have reached the player, they enter the Movement Attack phase
        if (Vector3.Distance(trsf.position, character.transform.position) <= Mathf.Abs(character.transform.position.y - trsf.position.y) + enemy.range)
        {
            character.SetState(new EnemyAttack(character, level, player));
        }

        Separate(level.characters);

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
                if (other.Equals(this)) break;

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
