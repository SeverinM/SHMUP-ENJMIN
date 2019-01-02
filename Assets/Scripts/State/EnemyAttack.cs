using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chaque ennemi tire sur le joueur selon une période de temps
/// </summary>
public class EnemyAttack : State
{
    private Enemy enemy;

    private float lastTime;
    private float shoots = 0;
    Queue<WaypointElement> elements;

    public EnemyAttack(Character character, Queue<WaypointElement> elt) : base(character)
    {
        enemy = character.GetComponent<Enemy>();
        elements = elt;
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
        character.SetState(new EnemyMovement(character, elements));
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        // Lance une attaque selon la periode
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
