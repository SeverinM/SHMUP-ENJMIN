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
        // Si le leader à été détruit, alors on se déplace aléatoirement
        if(enemy.leader == null)
        {
            enemy.FollowRandomPath();
        }

        deltaPosition = enemy.leader.transform.position - character.transform.position;

        // Si les ennemis ont atteint le joueur, ils rentrent dasn une phase d'attaque
        if (Vector3.Distance(enemy.leader.transform.position, character.transform.position) <= Mathf.Abs(character.transform.position.y - enemy.leader.transform.position.y) + enemy.AttackRange)
        {
            character.SetState(new EnemyAttack(character,allElems));
        }

        character.Separate();

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

}
