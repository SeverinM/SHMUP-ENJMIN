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

    private Level level;

    public EnemyAttack(Character character, Level level) : base(character)
    {
        enemy = character.GetComponent<Enemy>();
        this.level = level;

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
        //Wander 
        character.SetState(null);
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
            lastTime += enemy.shootPeriod;

            enemy.Rotate(level.Player);
            enemy.Shoot();
            shoots++;

            if (shoots == enemy.shootAmount)
            {
                NextState();
            }
        }
    }
}
