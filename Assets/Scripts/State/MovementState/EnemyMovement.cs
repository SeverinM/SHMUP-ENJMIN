using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permet à un joueur de se déplacer vers une Transform (un obstacle, un enemy ou le joueur)
/// </summary>
public class EnemyMovement : State
{
    Transform trsf;
    Vector3 targetPosition;

    Enemy enemy;

    Level level;

    Vector3 deltaPosition;

    Queue<WaypointElement> allElems;

    public EnemyMovement(Character chara, Level level, Transform trsf, Queue<WaypointElement> elt) : base(chara)
    {
        allElems = elt;
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

        // Si les ennemis ont atteint le joueur, ils rentrent dasn une phase d'attaque
        if (Vector3.Distance(trsf.position, character.transform.position) <= Mathf.Abs(character.transform.position.y - trsf.position.y) + enemy.attackRange)
        {
            character.SetState(new EnemyAttack(character, level, allElems));
        }

        Separate(level.characters);

        character.Rotate(level.Player);
        character.Move(deltaPosition.normalized);
    }

    public override void StartState()
    {
        character.OnTriggerExitChar += TriggerExit;
    }

    public override void EndState()
    {
        character.OnTriggerExitChar -= TriggerExit;
    }

    public void TriggerExit(Collider coll)
    {
        if (coll.tag == "FollowParent")
        {
            character.SetState(new FollowPathMovement(character, level, allElems, ((Enemy)character).Waypoints.loop));
        }
    }

    //Permet de separer les ennemies entre eux pour eviter qu'ils se marchent dessus
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
