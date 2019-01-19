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
    Transform target;
    Vector3 deltaPosition;
    Queue<WaypointElement> allElems;
    bool followLeader;
    float timeBegin = 0;
    Vector3 angle;

    public EnemyMovement(Character chara, Transform tar, bool followLeader = false) : base(chara)
    {
        enemy = (Enemy)character;
        target = tar;
        this.followLeader = followLeader;
    }

    public EnemyMovement(Character chara, Transform tar, Queue<WaypointElement> elt, bool followLeader = false) : base(chara)
    {
        allElems = elt;
        enemy = (Enemy)character;
        target = tar;
        this.followLeader = followLeader;
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void UpdateState()
    {
        timeBegin += (Time.deltaTime * character.PersonalScale * character.GetScale() * character.MoveSpeed) / character.CoeffRotation;
        //L'ennemi est freeze / en pause , il n'est pas supposé agir
        if (character.PersonalScale == 0 || target == null)
        {
            return;
        }

        //S'il tire sur quelqu'un
        if (followLeader && target.GetComponent<Character>().Context.ValuesOrDefault<Transform>("Target",null) != null)
        {
            character.SetState(new EnemyAttack(character, allElems, target.GetComponent<Character>().Context.ValuesOrDefault<Transform>("Target", null)));
        }

        // Si la cible à été détruit, alors on se déplace aléatoirement
        if (target == null)
        {
            enemy.FollowRandomPath();
            return;
        }

        deltaPosition = target.transform.position - character.transform.position;
        deltaPosition = new Vector3(deltaPosition.x, 0, deltaPosition.z);

        // L'ennemie est proche du joueur
        if (target.GetComponent<Player>() != null && Vector3.Distance(target.transform.position, character.transform.position) <= enemy.AttackRange)
        {
            character.SetState(new EnemyAttack(character, allElems, target.transform));
        }

        //La vitesse du personnage est de plus en plus lente au fur et a mesure qu'il s'approche de son leader pour eviter de lui rentrer dedans
        if (target.GetComponent<Player>() == null && character.PersonalScale > 0)
            character.PersonalScale = Mathf.Clamp((Vector3.Distance(target.transform.position, character.transform.position) / 3) / enemy.MoveSpeed, 0, 1);

        if (character.PersonalScale * character.GetScale() > 0)
            character.transform.forward = Vector3.Lerp(angle, deltaPosition, timeBegin);

        character.Move(character.Separate(deltaPosition.normalized));
    }

    public override void StartState()
    {
        character.OnTriggerExitChar += TriggerExit;
        angle = character.transform.forward;
    }

    public override void EndState()
    {
        character.OnTriggerExitChar -= TriggerExit;
    }

    public void TriggerExit(Collider coll)
    {
        //Quitter la zone du joueur n'a de sens que si l'ennemi poursuivait le joueur
        if (!followLeader && coll.tag == "FollowParent")
        {
            if (allElems == null) {
                character.SetState(new EnemyMovement(character, target, true));
            } else
            {
                character.SetState(new FollowPathMovement(character, allElems, ((Enemy)character).Waypoints.loop));
            }
        }
    }

    public override string GetName()
    {
        return "EnemyMovement";
    }

}
