using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permet à un joueur de se déplacer vers une Transform (un obstacle, un enemy ou le joueur)
/// </summary>
public class EnemyMovement : State
{
    Vector3 targetPosition;

    Enemy enemy;

    Level level;

    Vector3 deltaPosition;

    Queue<WaypointElement> allElems;

    public EnemyMovement(Character chara, Queue<WaypointElement> elt) : base(chara)
    {
        allElems = elt;
        level = character.Context.ValuesOrDefault<Level>("Level", null);
        enemy = character.GetComponent<Enemy>();
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void UpdateState()
    {
        deltaPosition = enemy.leader.transform.position - character.transform.position;

        // Si les ennemis ont atteint le joueur, ils rentrent dasn une phase d'attaque
        if (Vector3.Distance(enemy.leader.transform.position, character.transform.position) <= Mathf.Abs(character.transform.position.y - enemy.leader.transform.position.y) + enemy.AttackRange)
        {
            character.SetState(new EnemyAttack(character,allElems));
        }

        Separate();

        Player plyr = character.RaiseTryReaching();
        if (plyr != null)
        {
            character.Rotate(plyr.gameObject);
        }
        character.Move(deltaPosition.normalized);
        character.transform.forward = deltaPosition;
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
            character.SetState(new FollowPathMovement(character, allElems, ((Enemy)character).Waypoints.loop));
        }
    }


    //Permet de separer les ennemies entre eux pour eviter qu'ils se marchent dessus
    void Separate()
    {
        float desiredseparation = 3f;
        List<GameObject> characters = new List<GameObject>();
        foreach (RaycastHit hit in Physics.SphereCastAll(character.transform.position, desiredseparation, Vector3.zero))
        {
            if (hit.collider.GetComponent<Enemy>() != null)
            {
                characters.Add(hit.collider.gameObject);
            }
        }

        float maxForce = 2f;
        Vector3 sum = new Vector3();
        int count = 0;

        foreach (GameObject other in characters)
        {
            if (other.Equals(this)) break;

            float d = Vector3.Distance(character.transform.position, other.transform.position);
            Vector3 diff = character.transform.position - other.transform.position;
            diff.Normalize();
            diff /= d;
            sum += diff;
            count++;
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
