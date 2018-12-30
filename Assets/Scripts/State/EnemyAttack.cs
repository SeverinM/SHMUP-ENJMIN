using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Each enemy can shoot the player according to a shoot period
/// </summary>
public class EnemyAttack : State
{
    private Enemy enemy;

    private float lastTime;
    private float shoots = 0;
    Transform target;
    Queue<WaypointElement> elements;

    public EnemyAttack(Character character, Queue<WaypointElement> elt) : base(character)
    {
        enemy = character.GetComponent<Enemy>();
        elements = elt;
        target = character.Context.ValuesOrDefault<Transform>("Target", null);
        lastTime = Time.time;
    }
    
    public override void EndState()
    {
        base.EndState();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        character.SetState(new EnemyMovement(character, target, elements));
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        // Attacks according to shootPeriod
        if (lastTime < Time.time)
        {
            lastTime += enemy.ShootPeriod;

            Player plyr = character.RaiseTryReaching();
            if (plyr != null)
            {
                enemy.Rotate(plyr.gameObject);
            }

            enemy.Shoot();
            shoots++;

            if (shoots == enemy.ShootAmount)
            {
                NextState();
            }
        }
    }
}
